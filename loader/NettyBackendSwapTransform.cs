using System;
using System.Collections.Generic;
using org.objectweb.asm;
using org.objectweb.asm.commons;

/// <summary>
/// Swaps Minecraft's netty transport backends out for IKVM/Wasm-friendly ones:
///
/// <list type="bullet">
///   <item><description>Real TCP sockets: <c>NioEventLoopGroup</c> + <c>NioSocketChannel</c>
///   become <c>OioEventLoopGroup</c> + <c>OioSocketChannel</c>. NIO needs working
///   <c>Selector</c>s, which depend on epoll/kqueue/select syscalls — OIO uses
///   plain blocking <c>java.net.Socket</c> which is what we have on this runtime.</description></item>
///   <item><description>Memory channel (single-player integrated server):
///   <c>NioEventLoopGroup</c> becomes <c>DefaultEventLoopGroup</c>.
///   <c>LocalServerChannel</c> doesn't do real I/O so it works fine with the
///   plain executor-style loop, and we avoid pulling in NIO at all.</description></item>
///   <item><description><c>Epoll.isAvailable()</c> is shimmed to always return
///   <c>false</c>, which kills the dead epoll branches before they can resolve
///   <c>EpollEventLoopGroup</c> / <c>EpollSocketChannel</c> (those reference
///   native epoll bindings we can't link).</description></item>
/// </list>
///
/// Patches three classes:
/// <list type="number">
///   <item><description><c>net.minecraft.network.Connection</c> — client TCP
///   connect path + the <c>LazyLoadedValue</c> supplier lambda that builds
///   <c>NETWORK_WORKER_GROUP</c>.</description></item>
///   <item><description><c>net.minecraft.client.multiplayer.ServerStatusPinger</c>
///   — the legacy-server ping path uses <c>NioSocketChannel</c> directly.</description></item>
///   <item><description><c>net.minecraft.server.network.ServerConnectionListener</c>
///   — the supplier for <c>SERVER_EVENT_GROUP</c>, which is what the
///   <c>startMemoryChannel</c> path uses for the local <c>LocalServerChannel</c>.
///   The TCP listener path (Open to LAN) is unreachable in a browser client and
///   left alone.</description></item>
/// </list>
///
/// The type substitution runs through ASM's <see cref="ClassRemapper"/> so that
/// every reference is updated consistently — type insns, method/field owners,
/// method descriptors (including return types of the synthetic supplier
/// lambdas, where a partial rewrite would otherwise produce a
/// <c>VerifyError</c>), invokedynamic bootstrap args, class-literal LDCs, and
/// signature attributes. Constructor descriptors line up across the swap pairs
/// (<c>(I;Ljava/util/concurrent/ThreadFactory;)V</c> exists on all three event
/// loop groups; the <c>SocketChannel</c> swaps are class-literal-only), so no
/// extra bytecode surgery is needed beyond the remap.
/// </summary>
internal static class NettyBackendSwapTransform
{
    public sealed class Mapping
    {
        public string ConnectionInternalName { get; init; }
        public string ServerStatusPingerInternalName { get; init; }
        public string ServerConnectionListenerInternalName { get; init; }
    }

    /// <summary>1.16.1 obfuscated names (from <c>mappings_client_1.16.1.txt</c>).</summary>
    public static readonly Mapping Obfuscated_1_16_1 = new()
    {
        ConnectionInternalName = "me",
        ServerStatusPingerInternalName = "dyu",
        ServerConnectionListenerInternalName = "zu",
    };

    /// <summary>Deobfuscated / Mojang-mapped names (match the decompiled source).</summary>
    public static readonly Mapping Deobfuscated = new()
    {
        ConnectionInternalName = "net/minecraft/network/Connection",
        ServerStatusPingerInternalName = "net/minecraft/client/multiplayer/ServerStatusPinger",
        ServerConnectionListenerInternalName = "net/minecraft/server/network/ServerConnectionListener",
    };

    private const string NioEventLoopGroup = "io/netty/channel/nio/NioEventLoopGroup";
    private const string OioEventLoopGroup = "io/netty/channel/oio/OioEventLoopGroup";
    private const string DefaultEventLoopGroup = "io/netty/channel/DefaultEventLoopGroup";
    private const string NioSocketChannel = "io/netty/channel/socket/nio/NioSocketChannel";
    private const string OioSocketChannel = "io/netty/channel/socket/oio/OioSocketChannel";
    private const string EpollOwner = "io/netty/channel/epoll/Epoll";

    /// <summary>
    /// Produces one transformer entry per patched class. Spread the result into
    /// <see cref="MinecraftLaunchOptions.AsmTransformers"/>.
    /// </summary>
    public static IReadOnlyList<(Predicate<string> Filter, Func<ClassWriter, ClassVisitor> BuildVisitor)> AsTransformers(Mapping mapping)
    {
        return
        [
            MakeTransformer(
                mapping.ConnectionInternalName,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [NioEventLoopGroup] = OioEventLoopGroup,
                    [NioSocketChannel] = OioSocketChannel,
                },
                shimEpollIsAvailable: true),

            MakeTransformer(
                mapping.ServerStatusPingerInternalName,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [NioSocketChannel] = OioSocketChannel,
                },
                shimEpollIsAvailable: false),

            MakeTransformer(
                mapping.ServerConnectionListenerInternalName,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [NioEventLoopGroup] = DefaultEventLoopGroup,
                },
                shimEpollIsAvailable: true),
        ];
    }

    private static (Predicate<string>, Func<ClassWriter, ClassVisitor>) MakeTransformer(
        string classInternalName,
        Dictionary<string, string> typeRemap,
        bool shimEpollIsAvailable)
    {
        var dottedName = classInternalName.Replace('/', '.');
        return (
            name => string.Equals(name, dottedName, StringComparison.Ordinal),
            writer =>
            {
                Console.WriteLine($"[NettyBackendSwapTransform] patching {classInternalName}: {typeRemap.Count} type remap(s){(shimEpollIsAvailable ? ", Epoll.isAvailable -> false" : "")}");

                var remapTable = new java.util.HashMap();
                foreach (var pair in typeRemap)
                {
                    remapTable.put(pair.Key, pair.Value);
                }

                // ClassRemapper rewrites every type reference (insns, owners,
                // descriptors, signatures, invokedynamic bootstrap args, LDC
                // type constants) — without this the synthetic supplier
                // lambdas keep their original return-type descriptor and the
                // verifier rejects the rewritten body.
                ClassVisitor chain = new ClassRemapper(writer, new SimpleRemapper(remapTable));

                // The Epoll shim sits in front of the remapper so the
                // INVOKESTATIC is dropped before the remapper has a chance to
                // walk it. Functionally equivalent to ordering it after, since
                // we don't remap Epoll itself — purely a clarity choice.
                if (shimEpollIsAvailable)
                {
                    chain = new EpollIsAvailableShim(chain);
                }

                return chain;
            }
        );
    }

    /// <summary>
    /// Replaces <c>INVOKESTATIC io/netty/channel/epoll/Epoll.isAvailable ()Z</c>
    /// with <c>ICONST_0</c>. Same stack effect (push one int), and short-circuits
    /// every <c>Epoll.isAvailable() &amp;&amp; ...</c> chain into its else branch
    /// without ever resolving <c>Epoll.&lt;clinit&gt;</c> (which would load native
    /// bindings we don't have on Wasm).
    /// </summary>
    private sealed class EpollIsAvailableShim : ClassVisitor
    {
        public EpollIsAvailableShim(ClassVisitor downstream)
            : base(Opcodes.ASM9, downstream) { }

        public override MethodVisitor visitMethod(int access, string name, string descriptor, string signature, string[] exceptions)
        {
            var inner = base.visitMethod(access, name, descriptor, signature, exceptions);
            if (inner is null)
            {
                return null;
            }
            return new ShimMethodVisitor(inner);
        }

        private sealed class ShimMethodVisitor : MethodVisitor
        {
            public ShimMethodVisitor(MethodVisitor downstream)
                : base(Opcodes.ASM9, downstream) { }

            public override void visitMethodInsn(int opcode, string owner, string name, string descriptor, bool isInterface)
            {
                if (opcode == Opcodes.INVOKESTATIC
                    && owner == EpollOwner
                    && name == "isAvailable"
                    && descriptor == "()Z")
                {
                    base.visitInsn(Opcodes.ICONST_0);
                    return;
                }

                base.visitMethodInsn(opcode, owner, name, descriptor, isInterface);
            }
        }
    }
}
