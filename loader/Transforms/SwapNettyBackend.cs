using System.Linq;
using org.objectweb.asm;
using org.objectweb.asm.commons;

internal class SwapNettyBackendTransform : ClassRemapper
{
	private const string McConnection = "net.minecraft.network.Connection";
	private const string McServerStatusPinger = "net.minecraft.client.multiplayer.ServerStatusPinger";
	private const string McServerConnectionListener = "net.minecraft.server.network.ServerConnectionListener";

    private const string NioEventLoopGroup = "io/netty/channel/nio/NioEventLoopGroup";
    private const string OioEventLoopGroup = "io/netty/channel/oio/OioEventLoopGroup";
    private const string DefaultEventLoopGroup = "io/netty/channel/DefaultEventLoopGroup";
    private const string NioSocketChannel = "io/netty/channel/socket/nio/NioSocketChannel";
    private const string OioSocketChannel = "io/netty/channel/socket/oio/OioSocketChannel";

	public static IkvmClassLoaderTransformer Transformer = () => {
		string[] classes = [McConnection, McServerStatusPinger, McServerConnectionListener];
		return (classes.Select(x => Mappings.CurrentMappings.GetClass(x).Name).ToArray(), typeof(SwapNettyBackendTransform));
	};

	private static SimpleRemapper RemapperForClass(string name)
	{
		java.util.HashMap map = new();

		if (name == Mappings.CurrentMappings.GetClass(McServerConnectionListener).Name)
		{
			map.put(NioEventLoopGroup, DefaultEventLoopGroup);
		}
		else
		{
			if (name == Mappings.CurrentMappings.GetClass(McConnection).Name)
				map.put(NioEventLoopGroup, OioEventLoopGroup);
			map.put(NioSocketChannel, OioSocketChannel);
		}

		return new(map);
	}

	public SwapNettyBackendTransform(string name, ClassWriter writer) : base(writer, RemapperForClass(name))
	{
	}
}
