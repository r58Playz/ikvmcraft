STATICS_RELEASE=87d20f43-0820-47e0-8cf1-b4e0cd1a50aa
DOTNETFLAGS=--nodereuse:false -v n

statics:
	mkdir statics
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/dotnet.zip -O statics/dotnet.zip

emsdk:
	git clone https://github.com/emscripten-core/emsdk
	./emsdk/emsdk install 3.1.56
	./emsdk/emsdk activate 3.1.56
	python3 ./sanitizeemsdk.py "$(shell realpath ./emsdk/)"
	patch -p1 --directory emsdk/upstream/emscripten/ < emsdk.patch
	patch -p1 --directory emsdk/upstream/emscripten/ < emsdk.2.patch
	rm -rvf emsdk/upstream/emscripten/cache/*

dotnetclean:
	rm -rvf {bin,obj} || true
clean: dotnetclean
	rm -rvf statics || true

deps: statics emsdk

build: deps
	rm -r frontend/public/_framework bin/Release/net10.0/publish/wwwroot/_framework statics/dotnet || true
	unzip -q -o statics/dotnet.zip -d statics/dotnet
	dotnet publish -c Release $(DOTNETFLAGS)
	cp -r bin/Release/net10.0/publish/wwwroot/_framework frontend/public/
	# emscripten sucks
	sed -i 's/var offscreenCanvases \?= \?{};/var offscreenCanvases={};if(globalThis.window\&\&!window.TRANSFERRED_CANVAS){transferredCanvasNames=[".canvas"];window.TRANSFERRED_CANVAS=true;}/' frontend/public/_framework/dotnet.native.*.js
	# dotnet messed up
	sed -i 's/this.appendULeb(32768)/this.appendULeb(65535)/' frontend/public/_framework/dotnet.runtime.*.js

serve: build
	cd frontend && pnpm dev

publish: build
	cd frontend && pnpm build


.PHONY: clean build serve publish
