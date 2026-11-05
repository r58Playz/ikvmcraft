# ikvmcraft

Modern Minecraft + drop-in mods loaded dynamically in the browser with mono-wasm and IKVM. Uses a [custom fork of IKVM](https://github.com/r58Playz/ikvm/tree/wasm) with exact OpenJDK classloading semantics and other wasm patches, as well as a [custom fork of mono](https://github.com/r58Playz/dotnet-runtime/tree/wasm-jspi-jit-10.0.3) with a full wasm JIT tier significantly improving performance over interp/jiterp.

![Minecraft 26.2 with Fabulously Optimized modpack](assets/26.2-fabulouslyoptimized.png)
Minecraft 26.2 with Fabulously Optimized modpack

![Minecraft 1.16.5 in-game with 24 render distance](assets/1.16.5-24render.png)
Minecraft 1.16.5 in-game with 24 render distance

![Minecraft 1.16.5 in-game with BSL Medium & 12 render distance](assets/1.16.5-bsl.png)
Minecraft 1.16.5 in-game with BSL Medium & 12 render distance

Supports Minecraft 1.13-26.2 with Fabric 0.19.x (other Fabric versions not tested but may work). Java runtime is mostly Java 25 compliant. Uses OpenGL 4.6 on top of WebGPU.

NOT AN OFFICIAL MINECRAFT PRODUCT. NOT APPROVED BY OR ASSOCIATED WITH MOJANG OR MICROSOFT.

## building

1. download dotnet 10, a jdk, and potentially also mono. be on linux.
2. run `sudo dotnet workload restore` in `loader` folder
3. run `make build` in this folder to build
4. run `make serve` to serve a DEVELOPMENT version that SHOULD NOT BE HOSTED PUBLICLY.
5. run `make publish` to build a PRODUCTION version in `frontend/dist` folder.

