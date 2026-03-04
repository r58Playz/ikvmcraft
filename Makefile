STATICS_RELEASE=ed859c6f-413e-40ba-88a0-930cd947eff0
IKVM_RELEASE=fe1cbfa7-fc81-4135-a7d3-b808f7485136
DOTNETFLAGS=--nodereuse:false -v n

statics:
	mkdir statics
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/dotnet.zip -O statics/dotnet.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/ikvm-wasm-bundle.zip -O statics/ikvm.zip

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
	rm -r statics/{dotnet,ikvm} frontend/public/{_framework,ikvm} bin/Release/net10.0/publish/wwwroot/_framework || true
	unzip -q -o statics/dotnet.zip -d statics/dotnet
	unzip -q -o statics/ikvm.zip -d statics/ikvm
	dotnet publish -c Release $(DOTNETFLAGS)
	cp -r bin/Release/net10.0/publish/wwwroot/_framework frontend/public/
	cp -r statics/ikvm/image frontend/public/
	# emscripten sucks
	#sed -i 's/var offscreenCanvases \?= \?{};/var offscreenCanvases={};if(globalThis.window\&\&!window.TRANSFERRED_CANVAS){transferredCanvasNames=[".canvas"];window.TRANSFERRED_CANVAS=true;}/' frontend/public/_framework/dotnet.native.*.js
	# dotnet messed up
	sed -i 's/this.appendULeb(32768)/this.appendULeb(65535)/' frontend/public/_framework/dotnet.runtime.*.js

serve: build
	cd frontend && pnpm dev

publish: build
	cd frontend && pnpm build

testjar:
	mkdir -p frontend/public/assets/.testjar/build
	javac -d frontend/public/assets/.testjar/build test/Hello.java
	jar cfe frontend/public/assets/main.jar Hello -C frontend/public/assets/.testjar/build .
	rm -rf frontend/public/assets/.testjar


.PHONY: clean build serve publish testjar
