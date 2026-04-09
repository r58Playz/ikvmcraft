# ikvm wasm

python3 gen-static-libs.py /ikvm/bin/libiava.so:out/native/libiava.a /ikvm/bin/libzip.so:out/native/libzip.a /ikvm/bin/libnio.so:out/native/libnio.a /ikvm/bin/libnet.so:out/native/libnet.a /tmp/lwjgl/liblwjgl.so:../native-deps/out/mt/liblwjgl3.a /tmp/lwjgl/libglfw.so:../native-deps/out/mt/libglfw3.a /tmp/lwjgl/libjemalloc.so:../native-deps/out/mt/libjemalloc.a --rename-symbol /tmp/lwjgl/libglfw.so:emscripten_glfw3_get_proc_address:glfwGetProcAddress > ../../../ikvm-wasm/statics.c
