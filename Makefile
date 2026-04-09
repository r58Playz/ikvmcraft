STATICS_RELEASE=1bd54b97-34da-4bcb-befe-e28734c02187
IKVM_RELEASE=6a303bdb-96fd-4757-8353-ffe022214a17
DOTNETFLAGS=--nodereuse:false -v n

statics:
	mkdir statics
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/dotnet.zip -O statics/dotnet.zip
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/emsdk.zip -O statics/emsdk.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/ikvm-wasm-bundle.zip -O statics/ikvm.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libglfw3-mt.a -O statics/libglfw3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lib_emscripten_glfw3-mt.js -O statics/lib_emscripten_glfw3.js
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lwjgl3-mt.jar -O statics/lwjgl3.jar
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl3-mt.a -O statics/liblwjgl3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl_stb-mt.a -O statics/liblwjgl_stb.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libjemalloc-mt.a -O statics/libjemalloc.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libffi-mt.a -O statics/libffi.a
	unzip -q -o statics/emsdk.zip -d statics/

dotnetclean:
	rm -rvf {bin,obj} || true
clean: dotnetclean
	rm -rvf statics || true

deps: statics

build: deps
	rm -r statics/{dotnet,ikvm} frontend/public/{_framework,ikvm} bin/Release/net10.0/publish/wwwroot/_framework || true
	unzip -q -o statics/dotnet.zip -d statics/dotnet
	unzip -q -o statics/ikvm.zip -d statics/ikvm
	dotnet publish -c Release $(DOTNETFLAGS)
	cp -r bin/Release/net10.0/publish/wwwroot/_framework frontend/public/
	cp -r statics/ikvm/image frontend/public/
	cp statics/lwjgl3.jar frontend/public/assets/
	# emscripten sucks
	sed -i 's/var offscreenCanvases \?= \?{};/var offscreenCanvases={};if(globalThis.window\&\&!window.TRANSFERRED_CANVAS){transferredCanvasNames=[".canvas"];window.TRANSFERRED_CANVAS=true;}/' frontend/public/_framework/dotnet.native.*.js
	# dotnet messed up
	sed -i 's/this.appendULeb(32768)/this.appendULeb(65535)/' frontend/public/_framework/dotnet.runtime.*.js

serve: build
	cd frontend && pnpm dev

publish: build
	cd frontend && pnpm build

.PHONY: clean build serve publish testjar
