#include <emscripten/wasmfs.h>
#include <emscripten/html5.h>
#include <stdbool.h>
#include <assert.h>
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

backend_t fetch_backend = NULL;

int mount_fetch(char *srcdir, char *dstdir) {
	if (!fetch_backend) fetch_backend = wasmfs_create_fetch_backend(srcdir);
	return wasmfs_create_directory(dstdir, 0777, fetch_backend);
}

int mount_fetch_file(char *path) {
	if (!fetch_backend) return -1;

	int ret = wasmfs_create_file(path, 0777, fetch_backend);
	if (ret >= 0)
		return close(ret);
	return ret;
}
