STATICS_RELEASE=b88ddafa-7409-4f79-8364-fa1c347aa6f6
IKVM_RELEASE=5dc2f3d4-c852-4725-b4fb-6c8990a54c50
DOTNETFLAGS=--nodereuse:false -v n
AOT?=false
OPT?=false

statics:
	mkdir statics
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/dotnet.zip -O statics/dotnet.zip
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/emsdk.zip -O statics/emsdk.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/ikvm-wasm-bundle.zip -O statics/ikvm.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libglfw3-ng-gl4es-mt.a -O statics/libglfw3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lib_emscripten_glfw3-ng-gl4es-mt.js -O statics/lib_emscripten_glfw3.js
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lwjgl3-mt.jar -O statics/lwjgl3.jar
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl3-mt.a -O statics/liblwjgl3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl_stb-mt.a -O statics/liblwjgl_stb.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl_openal_stubs-mt.a -O statics/liblwjgl_openal.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libffi-mt.a -O statics/libffi.a
	unzip -q -o statics/emsdk.zip -d statics/

deps: statics

ASM_VERSION=9.7.1
ASM_BASE=https://repo1.maven.org/maven2/org/ow2/asm

asm-jars:
	test -f jars/asm-$(ASM_VERSION).jar          || wget $(ASM_BASE)/asm/$(ASM_VERSION)/asm-$(ASM_VERSION).jar                   -O jars/asm-$(ASM_VERSION).jar
	test -f jars/asm-tree-$(ASM_VERSION).jar     || wget $(ASM_BASE)/asm-tree/$(ASM_VERSION)/asm-tree-$(ASM_VERSION).jar         -O jars/asm-tree-$(ASM_VERSION).jar
	test -f jars/asm-analysis-$(ASM_VERSION).jar || wget $(ASM_BASE)/asm-analysis/$(ASM_VERSION)/asm-analysis-$(ASM_VERSION).jar -O jars/asm-analysis-$(ASM_VERSION).jar
	test -f jars/asm-commons-$(ASM_VERSION).jar  || wget $(ASM_BASE)/asm-commons/$(ASM_VERSION)/asm-commons-$(ASM_VERSION).jar   -O jars/asm-commons-$(ASM_VERSION).jar

build: deps asm-jars
	rm -r statics/{dotnet,ikvm} frontend/public/{_framework,ikvm} loader/bin/Release/net10.0/publish/wwwroot/_framework || true
	unzip -q -o statics/dotnet.zip -d statics/dotnet
	unzip -q -o statics/ikvm.zip -d statics/ikvm
#
	./ikvmc.sh statics/ikvmc_lwjgl3.dll statics/lwjgl3.jar
	./ikvmc.sh jars/ikvmc_log4j.dll jars/log4j-core-2.17.1.jar jars/log4j-api-2.17.1.jar
	./ikvmc.sh jars/ikvmc_asm.dll jars/asm-$(ASM_VERSION).jar jars/asm-tree-$(ASM_VERSION).jar jars/asm-analysis-$(ASM_VERSION).jar jars/asm-commons-$(ASM_VERSION).jar
	./aotprofile.sh statics/ikvm_java.aotprofile statics/ikvm/IKVM.Java.dll java. sun.nio. sun.reflect. sun.misc. sun.util. sun.security. sun.net. com.sun.nio. ikvm. jdk.internal.
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
	rm -rvf loader/{bin,obj} statics/ikvm_java.aotprofile || true
ikvmclean:
	rm -rvf jars/ikvmc_{log4j,asm}.{dll,pdb} statics/ikvmc_lwjgl3.{dll,pdb} || true
clean: dotnetclean
	rm -rvf statics || true
