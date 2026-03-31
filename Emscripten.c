#include <emscripten/wasmfs.h>
#include <emscripten/html5.h>
#include <stdbool.h>
#include <assert.h>
#include <stdint.h>
#include <unistd.h>
#include <stdio.h>

extern int managed_em_loop_callback(void);

static void em_loop_wrapper() {
    if (!managed_em_loop_callback()) {
        emscripten_cancel_main_loop();
    }
}

void start_em_loop() {
    emscripten_set_main_loop(em_loop_wrapper, 0, 0);
}

int mount_opfs() {
	backend_t opfs = wasmfs_create_opfs_backend();
	int ret = wasmfs_create_directory("/libsdl", 0777, opfs);
	return ret;
}

backend_t fetch_backend[8] = { NULL };

int mount_fetch(int id, char *srcdir, char *dstdir) {
	fetch_backend[id] = wasmfs_create_fetch_backend(srcdir);
	return wasmfs_create_directory(dstdir, 0777, fetch_backend[id]);
}

int mount_fetch_file(int id, char *path) {
	if (!fetch_backend[id]) return -1;

	int ret = wasmfs_create_file(path, 0777, fetch_backend[id]);
	if (ret >= 0)
		return close(ret);
	return ret;
}

uint64_t wasm_icall_lii(uint32_t a, uint32_t b) { return 0; }
uint64_t wasm_icall_liii(uint32_t a, uint32_t b, uint32_t c) { return 0; }
uint32_t wasm_icall_iiiliii(uint32_t a, uint32_t b, uint64_t c, uint32_t d, uint32_t e, uint32_t f) { return 0; }
void wasm_icall_viil(uint32_t a, uint32_t b, uint64_t c) {}

void mono_threads_request_thread_dump (void);
EMSCRIPTEN_KEEPALIVE void perform_thread_dump() {
	mono_threads_request_thread_dump();
}
