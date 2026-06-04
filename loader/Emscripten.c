#include <emscripten/wasmfs.h>
#include <emscripten/html5.h>
#include <stdbool.h>
#include <assert.h>
#include <stdint.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/stat.h>

extern void krypton_proc_init();
void ikvm_gl_init() {
	krypton_proc_init();
}

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

int mount_fetch_dir(int id, char *path) {
	if (!fetch_backend[id]) return -1;

	return wasmfs_create_directory(path, 0777, fetch_backend[id]);
}

int mount_fetch_file(int id, char *path) {
	if (!fetch_backend[id]) return -1;

	int ret = wasmfs_create_file(path, 0777, fetch_backend[id]);
	if (ret >= 0)
		return close(ret);
	return ret;
}

void mono_threads_request_thread_dump (void);
EMSCRIPTEN_KEEPALIVE void perform_thread_dump() {
	mono_threads_request_thread_dump();
}

void classloader_debug(char* log) {
	EM_ASM({ console.debug(UTF8ToString($0)); }, log);
}

struct _MonoImage {
	int   ref_count;
	void *storage;
	char *raw_data;
	uint32_t raw_data_len;
	uint8_t dynamic : 1;
	uint8_t not_executable : 1;
	uint8_t uncompressed_metadata : 1;
	uint8_t metadata_only : 1;
	uint8_t checked_module_cctor : 1;
	uint8_t has_module_cctor : 1;
	uint8_t idx_string_wide : 1;
	uint8_t idx_guid_wide : 1;
	uint8_t idx_blob_wide : 1;
	uint8_t core_clr_platform_code : 1;
	uint8_t minimal_delta : 1;
	char *name;
	char *filename;
};
typedef struct _MonoImage MonoImage;
MonoImage* mono_assembly_get_image (void *assembly);

void classloader_set_mono_assembly_filename(void *assembly, char *name) {
	MonoImage *image = mono_assembly_get_image(assembly);
	image->filename = strdup(name);
}
