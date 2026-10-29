STATICS_RELEASE=f436b72b-adfb-4c28-9cd7-8e2493f8e815
IKVM_RELEASE=b9449e8c-10b5-49aa-a57a-365348d837fe
DOTNETFLAGS=--nodereuse:false -v n
AOT?=true
OPT?=false

statics:
	mkdir statics
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/dotnet-jspi-jit.zip -O statics/dotnet.zip
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/emsdk.zip -O statics/emsdk.zip
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/SDL3.a -O statics/SDL3.a
	wget https://github.com/r58Playz/FNA-WASM-Build/releases/download/$(STATICS_RELEASE)/libopenal.a -O statics/libopenal.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/ikvm-wasm-bundle.zip -O statics/ikvm.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libglfw3-mt.a -O statics/libglfw3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lib_emscripten_glfw3-mt.js -O statics/lib_emscripten_glfw3.js
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libwink-mt.a -O statics/libwink.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libwink_webgpu-mt.a -O statics/libwink_webgpu.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libwink_tint-mt.a -O statics/libwink_tint.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/wink-js-mt.zip -O statics/wink-js.zip
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/lwjgl3-mt.jar -O statics/lwjgl3.jar
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl3-mt.a -O statics/liblwjgl3.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/liblwjgl_stb-mt.a -O statics/liblwjgl_stb.a
	wget https://github.com/r58Playz/IKVM-WASM-Build/releases/download/$(IKVM_RELEASE)/libffi-mt.a -O statics/libffi.a
	unzip -q -o statics/emsdk.zip -d statics/
	unzip -q -o statics/wink-js.zip -d statics/wink-js/

ikvmc-bundles: statics
	unzip -q -o statics/dotnet.zip -d statics/dotnet
	unzip -q -o statics/ikvm.zip -d statics/ikvm
	python3 build-ikvmc.py all

deps: statics ikvmc-bundles

build: ikvmc-bundles
	rm -r frontend/public/{_framework,ikvm} loader/bin/Release/net10.0/publish/wwwroot/_framework || true
#
	dotnet publish loader/IkvmWasm.csproj -c Release -p:IkvmWasmEnableAot=$(AOT) -p:IkvmWasmEnableWasmOpt=$(OPT) $(DOTNETFLAGS)
	cp -r loader/bin/Release/net10.0/publish/wwwroot/_framework frontend/public/
	cp -r statics/ikvm/image frontend/public/
	# dotnet messed up
	sed -i 's/this.appendULeb(32768)/this.appendULeb(65535)/' frontend/public/_framework/dotnet.runtime.*.js
	python3 split-resources.py

serve: build
	cd frontend && pnpm dev

publish: build
	cd frontend && pnpm build

serve-publish: publish
	cd frontend && pnpm preview

dotnetclean:
	rm -rvf statics/dotnet loader/{bin,obj} loader/Generated.targets loader/ikvmc-manifest.g.json || true
ikvmclean:
	rm -rvf statics/ikvm {statics,jars}/ikvmc_*.{dll,pdb} {statics,jars}/*.aotprofile {statics,jars}/*.aotprofile.stamp loader/Generated.targets loader/ikvmc-manifest.g.json || true
clean: dotnetclean ikvmclean
	rm -rvf statics || true
