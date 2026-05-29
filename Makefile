STATICS_RELEASE=d1e58815-9732-47a1-8078-ab0c58712ec0
IKVM_RELEASE=e96e0434-0f4f-4623-8c89-e2634a67e9e6
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
		ikvm.runtime. ikvm.internal. \
		java.lang. java.util. java.nio. java.net. java.security. java.time. \
		sun.nio.fs. sun.nio.cs. com.sun.nio.zipfs. \
		sun.reflect. sun.misc.
	# probably need to tighten fastutil more
	./aotprofile.sh jars/ikvmc_fastutil-8.2.1.aotprofile jars/ikvmc_fastutil-8.2.1.dll \
		it.unimi.dsi.fastutil.objects. \
		it.unimi.dsi.fastutil.ints. \
		it.unimi.dsi.fastutil.longs. \
		it.unimi.dsi.fastutil.doubles. \
		it.unimi.dsi.fastutil.shorts. \
		it.unimi.dsi.fastutil.booleans.
#
	dotnet publish loader/IkvmWasm.csproj -c Release -p:IkvmWasmEnableAot=$(AOT) -p:IkvmWasmEnableWasmOpt=$(OPT) $(DOTNETFLAGS)
	cp -r loader/bin/Release/net10.0/publish/wwwroot/_framework frontend/public/
	cp -r statics/ikvm/image frontend/public/
	# dotnet messed up
	sed -i 's/this.appendULeb(32768)/this.appendULeb(65535)/' frontend/public/_framework/dotnet.runtime.*.js
	# emscripten sucks
	sed -i 's/var offscreenCanvases \?= \?{};/var offscreenCanvases={};if(globalThis.window\&\&!window.TRANSFERRED_CANVAS){transferredCanvasNames=[".canvas"];window.TRANSFERRED_CANVAS=true;}/' frontend/public/_framework/dotnet.native.*.js
	# event-driven drain of main-thread proxy queue on worker checkMailbox notifications (replaces manual setInterval pump)
	sed -i 's|if (cmd === "checkMailbox") {|if (cmd === "checkMailbox") { if (!ENVIRONMENT_IS_PTHREAD \&\& wasmExports \&\& wasmExports["emscripten_main_thread_process_queued_calls"]) { try { wasmExports["emscripten_main_thread_process_queued_calls"](); } catch (e) {} }|' frontend/public/_framework/dotnet.native.*.js

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
