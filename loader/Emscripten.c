#include <emscripten.h>
#include <emscripten/em_asm.h>
#include <emscripten/wasmfs.h>
#include <emscripten/html5.h>
#include <emscripten/threading.h>
#include <pthread.h>
#include <stdbool.h>
#include <assert.h>
#include <stdint.h>
#include <stdio.h>
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
void mono_interp_pgo_add_method (void *method);
void ikvm_pgo_add_method (void *method) {
	mono_interp_pgo_add_method(method);
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

/* === cross-thread mono log forwarding ===
 *
 * mono's log handler (MONO_LOG_LEVEL/MONO_LOG_MASK output, e.g. "gc", "aot") runs on
 * whichever thread emitted the message and routes to *that* thread's console. Managed
 * code, the GC/finalizer, and the jiterpreter all run on worker threads, so their logs
 * only appear in DevTools' worker consoles — which means you must keep DevTools attached
 * (and eat its slowdown) to see e.g. GC timing. This installs a replacement handler that
 * forwards worker-thread logs to the UI thread's console (async / non-blocking), where the
 * app's console proxy folds them into the in-page LogView. No DevTools, no attach cost.
 *
 * It is inert until you raise the log level: mono only invokes the handler for messages
 * that pass the active MONO_LOG_LEVEL + MONO_LOG_MASK filter, so installing it always is
 * free during normal play. Note: this only covers mono_trace()-style logging (the mask
 * channels), not raw stdout/printf from native libs. */
typedef void (*MonoLogCallback)(const char *log_domain, const char *log_level, const char *message, int fatal, void *user_data);
extern void mono_trace_set_log_handler (MonoLogCallback callback, void *user_data);
/* the runtime's own JS-backed logger; chaining to it preserves symbolication and the
 * worker fatal-error -> force_exit behaviour for error/critical messages. */
extern void mono_wasm_trace_logger (const char *log_domain, const char *log_level, const char *message, int fatal, void *user_data);

static void ikvm_thread_log_forward (const char *log_domain, const char *log_level, const char *message, int fatal, void *user_data) {
	// keep the runtime's existing behaviour (worker console + fatal handling) intact
	mono_wasm_trace_logger (log_domain, log_level, message, fatal, user_data);

	// the UI thread already logged via the call above; only forward from worker threads
	if (emscripten_is_main_runtime_thread ())
		return;

	int lvl = 0; // 0 log, 1 info, 2 warn, 3 error, 4 debug
	if (log_level) {
		if (!strcmp (log_level, "error") || !strcmp (log_level, "critical")) lvl = 3;
		else if (!strcmp (log_level, "warning")) lvl = 2;
		else if (!strcmp (log_level, "info")) lvl = 1;
		else if (!strcmp (log_level, "debug")) lvl = 4;
	}

	unsigned tid = (unsigned)(uintptr_t)pthread_self ();
	const char *dom = log_domain ? log_domain : "";
	const char *msg = message ? message : "";
	// strdup into the shared heap: the proxied call runs later, after mono may have freed
	// its buffer. The UI thread frees the copy once it has logged it.
	int n = snprintf (NULL, 0, "[t%x][MONO:%s] %s", tid, dom, msg);
	if (n < 0) return;
	char *buf = (char *)malloc ((size_t)n + 1);
	if (!buf) return;
	snprintf (buf, (size_t)n + 1, "[t%x][MONO:%s] %s", tid, dom, msg);

	MAIN_THREAD_ASYNC_EM_ASM ({
		var s = UTF8ToString($0);
		_free($0);
		var c = $1;
		(c == 3 ? console.error : c == 2 ? console.warn : c == 1 ? console.info : c == 4 ? console.debug : console.log)(s);
	}, buf, lvl);
}

EMSCRIPTEN_KEEPALIVE void install_thread_log_forwarder (void) {
	mono_trace_set_log_handler (ikvm_thread_log_forward, NULL);
}

/* === cross-thread jiterpreter trace-heat dump ===
 *
 * The jiterpreter's per-trace hit_count lives in a GLOBAL shared-memory table (jiterpreter.c
 * TraceInfo), incremented atomically by whichever thread runs each trace. The patched runtime
 * also stores each trace's owning MonoMethod there. So this reads heat aggregated across ALL
 * threads from one place — no stop-the-world, no per-worker JS, no emscripten proxy (a thread
 * spinning in the interpreter never services a proxy queue anyway).
 *
 * Run it on the managed (deputy) thread (via the IkvmWasm.DumpJiterpHeat JSExport) so the mono
 * metadata calls are safe; the report is forwarded to the UI console (async) so it lands in the
 * in-page log. "hot method with compiled=0" == a hot trace entry point the jiterpreter never
 * jitted == prime interpreter-time / inlining target. */
extern int    mono_jiterp_get_trace_count (void);
extern void  *mono_jiterp_get_trace_method (int trace_index);
extern double mono_jiterp_get_trace_hit_count (int trace_index);
extern int    mono_jiterp_get_trace_is_compiled (int trace_index);
extern int    mono_jiterp_get_trace_abort_reason (int trace_index);
// MINT opcode -> name (lock-free static-table read), to name a trace's abort opcode.
extern const char *mono_interp_opname (int op);
// Name resolution via LOCK-FREE field reads only (these just return ->name / ->name_space and
// never take the image/loader lock or allocate), so the dump is safe to run on the UI thread —
// unlike mono_method_get_full_name, which locks for signature/generic formatting and deadlocks
// when called off a coop-registered mono thread while the busy deputy holds the lock.
extern const char *mono_method_get_name (void *method);
extern void       *mono_method_get_class (void *method);
extern const char *mono_class_get_name (void *klass);
extern const char *mono_class_get_namespace (void *klass);

typedef struct { void *m; double hits; int traces; int compiled; int abort; } HeatAgg;
static int heat_cmp_method (const void *a, const void *b) {
	void *x = ((const HeatAgg *)a)->m, *y = ((const HeatAgg *)b)->m;
	return x < y ? -1 : x > y ? 1 : 0;
}
static int heat_cmp_hits (const void *a, const void *b) {
	double x = ((const HeatAgg *)a)->hits, y = ((const HeatAgg *)b)->hits;
	return x < y ? 1 : x > y ? -1 : 0;
}
// abort_reason code (see TraceInfo.abort_reason) -> printable string. >= 0 is a MINT opcode.
static const char *heat_abort_name (int code) {
	if (code >= 0) return mono_interp_opname (code);
	switch (code) {
		case -1: return "(small)";   // too small / reached end of body
		case -2: return "(big)";     // trace too big
		case -3: return "(other)";   // other string reason
		default: return "-";         // -100: never attempted
	}
}

EMSCRIPTEN_KEEPALIVE void dump_jiterp_trace_heat (int top_n) {
	int n = mono_jiterp_get_trace_count ();
	if (n <= 0) {
		MAIN_THREAD_ASYNC_EM_ASM ({ console.log("[jiterp heat] no traces yet"); });
		return;
	}

	HeatAgg *a = (HeatAgg *)malloc (sizeof (HeatAgg) * (size_t)n);
	if (!a) return;
	int cnt = 0;
	double total_hits = 0;
	for (int i = 0; i < n; i++) {
		void *m = mono_jiterp_get_trace_method (i);
		if (!m) continue;
		double h = mono_jiterp_get_trace_hit_count (i);
		a[cnt].m = m; a[cnt].hits = h; a[cnt].traces = 1;
		a[cnt].compiled = mono_jiterp_get_trace_is_compiled (i) ? 1 : 0;
		a[cnt].abort = mono_jiterp_get_trace_abort_reason (i);
		total_hits += h; cnt++;
	}

	// aggregate per method: sort by method ptr, merge adjacent runs
	qsort (a, (size_t)cnt, sizeof (HeatAgg), heat_cmp_method);
	int w = 0;
	for (int i = 0; i < cnt;) {
		int j = i; double h = 0; int t = 0, c = 0;
		// attribute the method's abort reason to its hottest entry point that never compiled
		int best_abort = -100; double best_abort_hits = -1;
		while (j < cnt && a[j].m == a[i].m) {
			h += a[j].hits; t += a[j].traces; c += a[j].compiled;
			if (!a[j].compiled && a[j].hits > best_abort_hits) { best_abort_hits = a[j].hits; best_abort = a[j].abort; }
			j++;
		}
		a[w].m = a[i].m; a[w].hits = h; a[w].traces = t; a[w].compiled = c; a[w].abort = best_abort; w++; i = j;
	}
	qsort (a, (size_t)w, sizeof (HeatAgg), heat_cmp_hits);
	if (top_n <= 0 || top_n > w) top_n = w;

	size_t cap = 8192, len = 0;
	char *buf = (char *)malloc (cap);
	if (!buf) { free (a); return; }
	#define HEAT_APPEND(...) do { \
		int need = snprintf (NULL, 0, __VA_ARGS__); \
		if (len + (size_t)need + 1 > cap) { while (len + (size_t)need + 1 > cap) cap *= 2; buf = (char *)realloc (buf, cap); } \
		len += (size_t)snprintf (buf + len, cap - len, __VA_ARGS__); \
	} while (0)

	HEAT_APPEND ("=== jiterp trace heat: %d methods / %d entry points, %.0f total entry-hits (all threads) ===\n", w, cnt, total_hits);
	HEAT_APPEND ("        hits   jit/eps  abort@        method  (jit<eps => hot; abort@ = why the hottest uncompiled entry failed)\n");
	for (int i = 0; i < top_n; i++) {
		void *k = mono_method_get_class (a[i].m);
		const char *mn = mono_method_get_name (a[i].m);
		const char *cn = k ? mono_class_get_name (k) : "?";
		const char *ns = k ? mono_class_get_namespace (k) : "";
		const char *ab = heat_abort_name (a[i].abort);
		// "ns.Class:method" from lock-free field reads; nothing to free (not allocations)
		HEAT_APPEND ("%12.0f   %3d/%-3d  %-13s %s%s%s:%s\n", a[i].hits, a[i].compiled, a[i].traces,
			ab, ns ? ns : "", (ns && ns[0]) ? "." : "", cn ? cn : "?", mn ? mn : "?");
	}
	#undef HEAT_APPEND
	free (a);

	// forward the whole report to the UI console (async, non-blocking); main frees the copy
	MAIN_THREAD_ASYNC_EM_ASM ({ var s = UTF8ToString($0); _free($0); console.log(s); }, buf);
}
