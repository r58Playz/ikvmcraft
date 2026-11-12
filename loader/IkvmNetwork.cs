using System;
using System.IO;

// Java's networking is the JDK's own (libnet, libnio) over WasmFS's sockets, epoll and resolver (FNA-WASM-Build
// emsdk.12); the TCP transport is wasmfs-wisp-transport.js. What is left here is the one thing libnet reads from a file.

// IKVM's libnet reports a synthetic eth0 under emscripten and reads its MAC from sysfs, as Linux does
// (emscripten-stubs/ikvm_netif.h in ikvm-wasm-build). Make one up per install and keep it in OPFS, so the hardware
// address -- and the hardware IDs mods derive from it -- is stable across launches but still differs between installs.
public static class IkvmNetworkInterfaces
{
	private const string PersistedAddressPath = "/libsdl/ikvmcraft/eth0-address";
	private const string SysfsAddressPath = "/sys/class/net/eth0/address";

	public static void Install()
	{
		if (!File.Exists(PersistedAddressPath))
		{
			var mac = System.Security.Cryptography.RandomNumberGenerator.GetBytes(6);
			mac[0] = (byte)((mac[0] & 0xfc) | 0x02); // unicast, locally administered
			Directory.CreateDirectory(Path.GetDirectoryName(PersistedAddressPath));
			File.WriteAllText(PersistedAddressPath, BitConverter.ToString(mac).Replace('-', ':').ToLowerInvariant() + "\n");
		}

		Directory.CreateDirectory(Path.GetDirectoryName(SysfsAddressPath));
		File.Copy(PersistedAddressPath, SysfsAddressPath, overwrite: true);
	}
}
