STATICS_RELEASE=04188551-4df9-4ffb-aa7b-fcada59156f1
IKVM_RELEASE=c9c54d90-5a40-427d-9109-df9dd9a1bd9a
EPOXY_BASE=https://puter-net.b-cdn.net/epoxy/f006127
DOTNETFLAGS=--nodereuse:false -v n
AOT?=false
OPT?=false

statics:
	mkdir statics
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/dotnet-jspi.zip -O statics/dotnet.zip
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/emsdk.zip -O statics/emsdk.zip
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/SDL3.a -O statics/SDL3.a
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/libopenal.a -O statics/libopenal.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/ikvm-wasm-bundle.zip -O statics/ikvm.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libglfw3-ng-gl4es-mt.a -O statics/libglfw3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lib_emscripten_glfw3-ng-gl4es-mt.js -O statics/lib_emscripten_glfw3.js
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lwjgl3-mt.jar -O statics/lwjgl3-3.2.2.jar
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl3-mt.a -O statics/liblwjgl3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl_stb-mt.a -O statics/liblwjgl_stb.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libffi-mt.a -O statics/libffi.a
	wget $(EPOXY_BASE)/full.js -O statics/epoxy-full.js
	wget $(EPOXY_BASE)/full.wasm -O statics/epoxy-full.wasm
	unzip -q -o statics/emsdk.zip -d statics/

deps: statics

ikvmc-bundles: deps
	unzip -q -o statics/dotnet.zip -d statics/dotnet
	unzip -q -o statics/ikvm.zip -d statics/ikvm
	python3 build-ikvmc.py all

build: ikvmc-bundles
	rm -r frontend/public/{_framework,ikvm} loader/bin/Release/net10.0/publish/wwwroot/_framework || true
#
	./aotprofile.sh statics/ikvm_java.aotprofile statics/ikvm/IKVM.Java.dll \
		ikvm. java.lang. java.util. java.io. java.nio. java.net. java.security. java.time. java.math. \
		sun.nio.fs. sun.nio.cs. com.sun.nio.zipfs. sun.net. \
		sun.reflect. sun.misc.
	./aotprofile.sh jars/ikvmc_fastutil-8.2.1.aotprofile jars/ikvmc_fastutil-8.2.1.dll \
		@fastutil-aot-classes.txt
#
	dotnet publish loader/IkvmWasm.csproj -c Release -p:IkvmWasmEnableAot=$(AOT) -p:IkvmWasmEnableWasmOpt=$(OPT) $(DOTNETFLAGS)
	cp -r loader/bin/Release/net10.0/publish/wwwroot/_framework frontend/public/
	cp -r statics/ikvm/image frontend/public/
	# dotnet messed up
	sed -i 's/this.appendULeb(32768)/this.appendULeb(65535)/' frontend/public/_framework/dotnet.runtime.*.js
	# emscripten sucks
	sed -i 's/var offscreenCanvases \?= \?{};/var offscreenCanvases={};if(globalThis.window\&\&!window.TRANSFERRED_CANVAS){transferredCanvasNames=[".canvas"];window.TRANSFERRED_CANVAS=true;}/' frontend/public/_framework/dotnet.native.*.js

serve: build
	cd frontend && pnpm dev

publish: build
	cd frontend && pnpm build

dotnetclean:
	rm -rvf loader/{bin,obj} loader/Generated.targets loader/ikvmc-manifest.g.json || true
ikvmclean:
	rm -rvf {statics,jars}/ikvmc_*.{dll,pdb} {statics,jars}/*.aotprofile loader/Generated.targets loader/ikvmc-manifest.g.json || true
clean: dotnetclean ikvmclean
	rm -rvf statics || true
