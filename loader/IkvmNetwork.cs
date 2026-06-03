using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using java.net;
using sun.net.spi.nameservice;

static class IkvmNetworkState
{
	static List<string> FakeDnsResolves = new();

	private static (byte, byte, byte, byte) GetIPFromIndex(int index)
	{
		if (index / byte.MaxValue > byte.MaxValue) throw new ArgumentOutOfRangeException("index too large");
		if (index < 0) throw new ArgumentOutOfRangeException("index below zero");

		return (240, 67, (byte)(index / byte.MaxValue), (byte)(index % byte.MaxValue));
	}
	private static int GetIndexFromIP((byte, byte, byte, byte) ip)
	{
		return ip.Item3 * byte.MaxValue + ip.Item4;
	}

	internal static (byte, byte, byte, byte) GetFakeIP(string host)
	{
		lock (FakeDnsResolves) {
			var idx = FakeDnsResolves.FindIndex(x => x == host);
			if (idx < 0) {
				idx = FakeDnsResolves.Count;
				FakeDnsResolves.Add(host);
			}
			return GetIPFromIndex(idx);
		}
	}

	internal static string GetHostFromFakeIP((byte, byte, byte, byte) ip)
	{
		lock(FakeDnsResolves) {
			return FakeDnsResolves[GetIndexFromIP(ip)];
		}
	}
}

public class IkvmNameService : NameService
{
	public static void Install()
	{
		var _touch = InetAddress.getByAddress([240, 0, 0, 0]);

		var field = typeof(InetAddress).GetField("nameServices", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, java.util.Arrays.asList((NameService)new IkvmNameService()));
	}

	public InetAddress[] lookupAllHostAddr(string host)
	{
		var (a, b, c, d) = IkvmNetworkState.GetFakeIP(host);
		return [InetAddress.getByAddress([a, b, c, d])];
	}

	public string getHostByAddr(byte[] addr)
	{
		var address = (addr[0], addr[1], addr[2], addr[3]);
		try {
			return IkvmNetworkState.GetHostFromFakeIP(address);
		} catch(Exception e) {
			Console.WriteLine($"[IkvmNameService] failed to resolve [{string.Join(", ", addr)}] {e}");
		}
		throw new UnknownHostException();
	}
}

internal sealed class ActionRunnable : java.lang.Runnable
{
	private readonly Action Body;
	internal ActionRunnable(Action body) => Body = body;
	public void run() => Body();
}

internal sealed class IkvmWispConnection
{
	private readonly ClientWebSocket Ws;
	private readonly CancellationTokenSource Life = new();
	private readonly Action<byte[]> OnBytes;
	private readonly Action OnEof;
	private readonly Action<Exception> OnError;

	private readonly object SendGate = new();
	private readonly Queue<byte[]> SendQueue = new();
	private bool Sending;

	internal IkvmWispConnection(ClientWebSocket ws, Action<byte[]> onBytes, Action onEof, Action<Exception> onError)
	{
		Ws = ws;
		OnBytes = onBytes;
		OnEof = onEof;
		OnError = onError;
	}

	internal void Start() => _ = ReceiveLoopAsync();

	private async Task ReceiveLoopAsync()
	{
		var buf = new byte[16384];
		try
		{
			while (!Life.IsCancellationRequested)
			{
				var res = await Ws.ReceiveAsync(new ArraySegment<byte>(buf), Life.Token).ConfigureAwait(false);
				if (res.MessageType == WebSocketMessageType.Close)
				{
					OnEof();
					return;
				}
				if (res.Count <= 0)
					continue;

				var chunk = new byte[res.Count];
				Array.Copy(buf, 0, chunk, 0, res.Count);
				OnBytes(chunk);
			}
		}
		catch (OperationCanceledException) { /* closing */ }
		catch (Exception e) { OnError(e); }
	}

	internal void Send(byte[] bytes)
	{
		lock (SendGate)
		{
			SendQueue.Enqueue(bytes);
			if (Sending)
				return;
			Sending = true;
		}
		_ = SendPumpAsync();
	}

	private async Task SendPumpAsync()
	{
		try
		{
			while (true)
			{
				byte[] next;
				lock (SendGate)
				{
					if (SendQueue.Count == 0)
					{
						Sending = false;
						return;
					}
					next = SendQueue.Dequeue();
				}
				await Ws.SendAsync(new ArraySegment<byte>(next), WebSocketMessageType.Binary, true, Life.Token).ConfigureAwait(false);
			}
		}
		catch (OperationCanceledException)
		{
			lock (SendGate) { Sending = false; }
		}
		catch (Exception e)
		{
			lock (SendGate) { Sending = false; }
			OnError(e);
		}
	}

	internal void Close()
	{
		try { Life.Cancel(); } catch { }
		try { Ws.Abort(); } catch { }
		try { Ws.Dispose(); } catch { }
	}
}

public sealed class IkvmWebSocketChannel : io.netty.channel.AbstractChannel
{
	private static readonly io.netty.channel.ChannelMetadata METADATA = new(false);
	private const int MaxReaderStackDepth = 8;

	private readonly io.netty.channel.ChannelConfig Config;
	private readonly java.lang.Runnable ReadTask;
	private readonly Queue<io.netty.buffer.ByteBuf> InboundBuffer = new();

	internal java.net.SocketAddress Remote;
	internal java.net.SocketAddress Local;
	private IkvmWispConnection Conn;
	private volatile bool Open = true;
	private volatile bool Active;
	private bool ReadInProgress;
	private int ReaderDepth;

	public IkvmWebSocketChannel() : base(null)
	{
		Config = new io.netty.channel.DefaultChannelConfig(this);
		ReadTask = new ActionRunnable(RunReadTask);
	}

	public override io.netty.channel.ChannelMetadata metadata() => METADATA;
	public override io.netty.channel.ChannelConfig config() => Config;
	public override bool isOpen() => Open;
	public override bool isActive() => Active;

	protected override io.netty.channel.AbstractChannel.AbstractUnsafe newUnsafe() => new IkvmWispUnsafe(this);

	// Accept any event loop (Default/Nio/Oio); we drive everything off WebSocket callbacks.
	protected override bool isCompatible(io.netty.channel.EventLoop loop) => true;

	protected override java.net.SocketAddress localAddress0() => Local;
	protected override java.net.SocketAddress remoteAddress0() => Remote;

	protected override void doBind(java.net.SocketAddress local) => Local = local;

	protected override void doDisconnect() => doClose();

	protected override void doClose()
	{
		Active = false;
		Open = false;
		var conn = Conn;
		Conn = null;
		conn?.Close();
		while (InboundBuffer.Count > 0)
		{
			try { InboundBuffer.Dequeue().release(); } catch { }
		}
	}

	protected override void doBeginRead()
	{
		if (ReadInProgress || !Active)
			return;
		if (InboundBuffer.Count == 0)
		{
			ReadInProgress = true;
			return;
		}
		DeliverInbound();
	}

	protected override void doWrite(io.netty.channel.ChannelOutboundBuffer outboundBuffer)
	{
		var conn = Conn;
		for (;;)
		{
			var msg = outboundBuffer.current();
			if (msg == null)
				break;

			if (msg is io.netty.buffer.ByteBuf buf)
			{
				int len = buf.readableBytes();
				if (len > 0 && conn != null)
				{
					var bytes = new byte[len];
					buf.getBytes(buf.readerIndex(), bytes);
					// Handed to the async send pump; the write promise completes optimistically.
					// (No netty-level write backpressure — fine for MC's modest packet volume.)
					conn.Send(bytes);
				}
				outboundBuffer.remove();
			}
			else
			{
				// MC only writes ByteBufs at this stage; reject anything unexpected.
				outboundBuffer.remove(new java.lang.UnsupportedOperationException("unsupported message type"));
			}
		}
	}

	// --- callbacks invoked by the Unsafe / IkvmWispConnection (all run on the event loop) ---

	internal void BeginConnected(ClientWebSocket ws)
	{
		Active = true;
		Conn = new IkvmWispConnection(
			ws,
			chunk => RunOnLoop(() => OnInboundBytes(chunk)),
			() => RunOnLoop(OnInboundEof),
			e => RunOnLoop(() => OnInboundError(e)));
	}

	internal void StartReceiving() => Conn?.Start();

	internal void RunOnLoop(Action action)
	{
		var loop = eventLoop();
		if (loop.inEventLoop())
			action();
		else
			loop.execute(new ActionRunnable(action));
	}

	private void OnInboundBytes(byte[] chunk)
	{
		if (!Active)
			return;
		var buf = Config.getAllocator().buffer(chunk.Length);
		buf.writeBytes(chunk);

		InboundBuffer.Enqueue(buf);
		if (ReadInProgress)
		{
			ReadInProgress = false;
			DeliverInbound();
		}
	}

	private void OnInboundEof()
	{
		if (!Open)
			return;
		var u = @unsafe();
		u.close(u.voidPromise());
	}

	private void OnInboundError(Exception e)
	{
		if (!Open)
			return;
		// Surface the failure to the pipeline, then close (fires channelInactive -> MC disconnect).
		pipeline().fireExceptionCaught(new java.io.IOException(e.Message));
		var u = @unsafe();
		u.close(u.voidPromise());
	}

	private void RunReadTask()
	{
		if (!Active || InboundBuffer.Count == 0)
			return;
		ReadFromBuffer();
	}

	// Bounce off the stack via the event loop if fireChannelRead re-enters us too deeply.
	private void DeliverInbound()
	{
		if (ReaderDepth >= MaxReaderStackDepth)
		{
			eventLoop().execute(ReadTask);
			return;
		}
		ReaderDepth++;
		try { ReadFromBuffer(); }
		finally { ReaderDepth--; }
	}

	private void ReadFromBuffer()
	{
		var handle = @unsafe().recvBufAllocHandle();
		handle.reset(Config);
		var pipe = pipeline();
		do
		{
			if (InboundBuffer.Count == 0)
				break;
			var received = InboundBuffer.Dequeue();
			int n = received.readableBytes();
			handle.attemptedBytesRead(n);
			handle.lastBytesRead(n);
			handle.incMessagesRead(1);
			pipe.fireChannelRead(received);
		}
		while (handle.continueReading());
		handle.readComplete();
		pipe.fireChannelReadComplete();
	}

	// The remote carries one of IkvmNameService's synthetic 240.67.x.y addresses; map it back
	// to the original hostname so the proxy dials the real server. Falls back to the address's
	// own hostname (e.g. a literal IP that never went through our resolver).
	internal static (string, int) ResolveTarget(java.net.SocketAddress remote)
	{
		var isa = (java.net.InetSocketAddress)remote;
		int port = isa.getPort();
		var addr = isa.getAddress();
		if (addr != null)
		{
			var ip = addr.getAddress();
			if (ip != null && ip.Length == 4)
			{
				try { return (IkvmNetworkState.GetHostFromFakeIP((ip[0], ip[1], ip[2], ip[3])), port); }
				catch { }
			}
			return (addr.getHostName(), port);
		}
		return (isa.getHostString(), port);
	}
}

internal sealed class IkvmWispUnsafe : io.netty.channel.AbstractChannel.AbstractUnsafe
{
	private readonly IkvmWebSocketChannel Chan;

	internal IkvmWispUnsafe(IkvmWebSocketChannel chan) : base(chan) => Chan = chan;

	public override void connect(java.net.SocketAddress remoteAddress, java.net.SocketAddress localAddress, io.netty.channel.ChannelPromise promise)
	{
		if (!promise.setUncancellable() || !ensureOpen(promise))
			return;

		try
		{
			if (localAddress != null)
				Chan.Local = localAddress;
			Chan.Remote = remoteAddress;

			var (host, port) = IkvmWebSocketChannel.ResolveTarget(remoteAddress);
			var uri = new Uri($"ws://wisp-tcp/{host}:{port}");

			var ws = new ClientWebSocket();
			int timeout = Chan.config().getConnectTimeoutMillis();
			var cts = new CancellationTokenSource();
			if (timeout > 0)
				cts.CancelAfter(timeout);

			ws.ConnectAsync(uri, cts.Token).ContinueWith(t =>
			{
				cts.Dispose();
				Chan.RunOnLoop(() => FinishConnect(ws, uri, t, promise));
			}, TaskScheduler.Default);
		}
		catch (Exception e)
		{
			promise.tryFailure(annotateConnectException(new ConnectException(e.Message), remoteAddress));
			closeIfClosed();
		}
	}

	private void FinishConnect(ClientWebSocket ws, Uri uri, Task connectTask, io.netty.channel.ChannelPromise promise)
	{
		if (!Chan.isOpen())
		{
			// Channel was closed while the handshake was in flight.
			try { ws.Abort(); ws.Dispose(); } catch { }
			promise.tryFailure(new java.nio.channels.ClosedChannelException());
			return;
		}

		if (connectTask.IsFaulted || connectTask.IsCanceled)
		{
			try { ws.Abort(); ws.Dispose(); } catch { }
			var reason = connectTask.IsCanceled ? "timed out" : connectTask.Exception?.GetBaseException().Message;
			var cause = new ConnectException($"wisp websocket connect to {uri} failed: {reason}");
			promise.tryFailure(annotateConnectException(cause, Chan.Remote));
			close(voidPromise());
			return;
		}

		bool wasActive = Chan.isActive();
		Chan.BeginConnected(ws);
		bool active = Chan.isActive();
		bool promiseSet = promise.trySuccess();
		if (!wasActive && active)
			Chan.pipeline().fireChannelActive();
		Chan.StartReceiving(); // begin pumping inbound frames only after channelActive
		if (!promiseSet)
			close(voidPromise());
	}
}
