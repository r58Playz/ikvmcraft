import { createState } from "dreamland/core";
import type { ModuleAPI, MonoConfig, RuntimeAPI } from "../dotnetdefs";
import { EpxTcpWs } from "../net";
import { opfsInvalidate } from "../opfs";
import { fetchResource } from "./resources.js";

const wasm: ModuleAPI = await eval(`import("/_framework/dotnet.js")`);
const dotnet = wasm.dotnet;
let runtime: RuntimeAPI;
let config: MonoConfig;
let exports: any;

export interface MinecraftAuthInfo {
	playerName: string;
	uuid: string;
	accessToken: string;
	userType: string;
}

export let dotnetState = createState({
	wasmMemory: 0,
	gcMemory: 0,
	gcHeapMemory: 0,
	nativeMemory: 0,
	heapMemory: 0,
	fps: 0,
	fpsAvg: 0,
});

export function getDlls(): (readonly [string, string])[] {
	const config = (wasm.dotnet as any).instance.config;
	const resources = [
		...(config.resources?.coreAssembly || []),
		...(config.resources?.assembly || []),
	];
	return resources.map((x) => [x.name, x.virtualPath] as const);
}

/*
 * Copy a file into the root of OPFS, which WASMFS mounts at /libsdl (see loader/Emscripten.c). Used to
 * provision bench.jar for the jbox2d benchmark so it does not have to be dropped in by hand through the
 * fs explorer -- a stale hand-uploaded jar silently benchmarks the previous build's code.
 *
 * Done as a plain browser API call before the runtime starts, so it is independent of when emscripten
 * gets around to mounting OPFS. Re-fetches whenever the size differs, which is enough to catch a
 * rebuilt jar without hashing it on every boot.
 */
export async function provisionOpfsFile(
	name: string,
	url: string
): Promise<void> {
	const root = await navigator.storage.getDirectory();

	const res = await fetch(url, { cache: "no-store" });
	if (!res.ok) {
		console.warn(`[opfs] ${url} -> ${res.status}, leaving OPFS ${name} alone`);
		return;
	}
	const bytes = new Uint8Array(await res.arrayBuffer());

	try {
		const existing = await (await root.getFileHandle(name)).getFile();
		if (existing.size === bytes.byteLength) {
			console.debug(
				`[opfs] ${name} already current (${bytes.byteLength} bytes)`
			);
			return;
		}
		console.debug(
			`[opfs] ${name} stale (${existing.size} -> ${bytes.byteLength} bytes), replacing`
		);
	} catch {
		console.debug(`[opfs] ${name} missing, writing ${bytes.byteLength} bytes`);
	}

	const writable = await (
		await root.getFileHandle(name, { create: true })
	).createWritable();
	opfsInvalidate();
	try {
		await writable.write(bytes);
	} finally {
		await writable.close();
	}
	console.debug(`[opfs] wrote ${name} (${bytes.byteLength} bytes)`);
}

/*
 * Query-param overrides, applied on top of the in-source defaults so a URL always wins. Three forms:
 *
 *   ?wj.overrides={"vcall_ways":1}         JSON blob of short wasm_jit knob names
 *   ?wj.vcall_ways=1                       one short knob per param -> MONO_WASM_JIT_VCALL_WAYS
 *   ?wj.MONO_GC_PARAMS=nursery-size=4m     a name already carrying an env prefix passes through as-is
 *   ?ro.jiterpreter-aot-table-size=3072    replaces the matching --<name>=<value> runtime option
 *   ?ro.no-jiterpreter-traces-enabled      valueless -> appends the bare --<name> flag
 *
 * All three exist because the experiment scripts under scratchpad/mcsr build the ?wj.<ENVVAR>= form
 * (gcstress.sh, verifyfix.sh, raiseshard.sh, raiseir.sh) while newer callers build ?wj.overrides=.
 * Dropping the per-knob form did not fail loudly -- mono ignores env vars it does not know and the
 * page ignored params it did not parse, so every arm of every matrix silently ran the SAME config.
 *
 * That is why each applied override prints a line: it is the only evidence an arm actually differed.
 * Note the line proves the *page* applied it, not that the runtime understood it -- a knob the build
 * predates is still silently ignored. See HARNESS-NOTES.md.
 */
function queryOverrides(prefix: string): [string, string][] {
	const out: [string, string][] = [];
	for (const [key, val] of new URLSearchParams(location.search)) {
		if (!key.startsWith(prefix) || key === "wj.overrides") continue;
		out.push([key.slice(prefix.length), val]);
	}
	return out;
}

function applyRuntimeOptionOverrides(options: string[]) {
	for (const [opt, val] of queryOverrides("ro.")) {
		const flag = val === "" ? `--${opt}` : `--${opt}=${val}`;
		const at = options.findIndex(
			(o) => o === `--${opt}` || o.startsWith(`--${opt}=`)
		);
		if (at >= 0) options[at] = flag;
		else options.push(flag);
		console.log(`[harness] runtime option: ${flag}`);
	}
}

export async function initDotnet(canvas: HTMLCanvasElement) {
	// emscripten proxy hackfix number 39847232303
	(globalThis as any).Atomics.waitAsync = undefined;

	// bench.jar is on the IKVM classpath at PreInit below, so it has to be in OPFS before that.
	await provisionOpfsFile("bench.jar", "/bench.jar");

	(globalThis as any).WebSocket = new Proxy(WebSocket, {
		construct(t, a, n) {
			const url = new URL(a[0]);
			if (url.hostname.startsWith("wisp-")) return new EpxTcpWs(url);

			return Reflect.construct(t, a, n);
		},
	});

	console.time("dotnet ");
	//(globalThis as any).GLFW3_DEBUG = true;

	let runtimeOptions = [
		// accept smaller traces earlier
		`--jiterpreter-minimum-trace-value=${10}`,
		`--jiterpreter-minimum-trace-hit-count=${1000}`,
		`--jiterpreter-back-branch-boost=${980}`, // make sure this is below trace hit count
		`--jiterpreter-minimum-distance-between-traces=${3}`,
		`--jiterpreter-trace-monitoring-period=${500}`,
		`--jiterpreter-trace-monitoring-max-average-penalty=${75}`,

		// increase jit function limits
		`--jiterpreter-wasm-bytes-limit=${64 * 1024 * 1024}`,
		`--jiterpreter-max-module-size=${64 * 1024 - 1}`,
		`--jiterpreter-table-size=${128 * 1024}`,
		`--jiterpreter-aot-table-size=${64 * 1024}`,

		//`--jiterpreter-stats-enabled`,
		//`--jiterpreter-count-bailouts`,
		//`--jiterpreter-estimate-heat`,

		//`--jiterpreter-direct-jit-calls=false`,

		//`--no-jiterpreter-jit-call-enabled`,
		//`--no-jiterpreter-interp-entry-enabled`,

		//`--no-jiterpreter-traces-enabled`,
	];

	applyRuntimeOptionOverrides(runtimeOptions);

	// run wink on main thread
	const winkGlMain =
		(new URLSearchParams(location.search).get("wj.WINK_GL_MAIN") ?? "1") !==
		"0";
	(window as any).WINK_GL_MAIN = winkGlMain;

	let builder = dotnet
		.withConfig({ pthreadPoolInitialSize: 16 })
		.withModuleConfig({
			onRuntimeInitialized(Module: any) {
				(globalThis as any).wasm = { Module, FS: Module.FS };
			},
		})
		.withResourceLoader((type: string, _name: string, defaultUri: string) => {
			if (type !== "dotnetwasm" && type !== "assembly") return null;
			return fetchResource(defaultUri, { credentials: "same-origin" }).then(
				(r) =>
					type === "dotnetwasm" && r.ok
						? new Response(r.body, { headers: { "Content-Type": "application/wasm" } })
						: r
			);
		})
		.withEnvironmentVariable("WINK_GL_MAIN", winkGlMain ? "1" : "0")
		.withEnvironmentVariable("GEX_WGSL_CACHE", "0")
		.withEnvironmentVariable("MONO_SLEEP_ABORT_LIMIT", "20000")
		.withEnvironmentVariable(
			"MONO_GC_PARAMS",
			"nursery-size=64m,tlab-size=64k,max-heap-size=3000m,default-allowance-ratio=2.0,major=marksweep-conc-par,minor=simple-par"
		)
		//.withEnvironmentVariable("MONO_GC_DEBUG", "binary-protocol=/gcproto:64m")
		//.withEnvironmentVariable("MONO_GC_DEBUG", "print-pinning,print-gchandles")

		//.withEnvironmentVariable("IKVM_TRACE_LOADCLASS", "1")
		//.withEnvironmentVariable("IKVM_TRACE_DEFINE", "1")
		//.withEnvironmentVariable("IKVM_TRACE_VERIFYLOADS", "1")
		//.withEnvironmentVariable("IKVM_VERIFY_LOADS", "shadow")
		//.withEnvironmentVariable("IKVM_TRACE_RESOLUTION_ERRORS", "1")
		.withEnvironmentVariable("IKVM_LAZY_CLASS_LOADING", "1")
		.withEnvironmentVariable("IKVM_LAZY_INPLACE", "1")
		.withEnvironmentVariable("IKVM_LAZY_SIG", "1")

		.withEnvironmentVariable("DOTNET_PROCESSOR_COUNT", String(Math.min(navigator.hardwareConcurrency || 4, 4)))
		.withEnvironmentVariable("MONO_LOG_LEVEL", "debug")
		.withEnvironmentVariable("MONO_LOG_MASK", "gc")
		//.withEnvironmentVariable("MONO_LOG_MASK", "aot")

		//.withEnvironmentVariable("IKVM_FROMCLASS_TRACE", "1")
		//.withEnvironmentVariable("IKVM_UNSAFE_OFFSET_TRACE", "1")
		.withRuntimeOptions(runtimeOptions);

	let wasm_jit = {
		raise_nogc: 1,

		/* Diagnostics. All default 0 in the runtime too; listed so a harness can flip them by name
		 * without having to know the env-var spelling. */
		stats: 0,
		verbose: 0,
		entrycensus: 0,
		profile_fast: 0,
		arity: 0,
		storeguard: 0,
		// objguard additionally gates the interp-boundary type-confusion diagnostics (WASM_JIT_BADREF_ARG
		// and mono_interp_isinst's probe) AND the caller-attribution store they read -- so turning it on
		// adds a `local.get; i32.const; i32.store` to every residual and delegate site in every module.
		// A debugging knob, not a measurement one; keep it 0 in any arm.
		objguard: 0,
		missedref: 0,
		refverify: 0,
	};

	let overrides = JSON.parse(
		new URLSearchParams(location.search).get("wj.overrides") || "{}"
	);

	// ?wj.<NAME>=<value>. A name that already carries an env prefix is passed straight through (so
	// MONO_GC_PARAMS / IKVM_* are reachable too, and mesa-wink's GEX_WGPU_* / MESA_* switches, which read the same
	// C environment); anything else is a short wasm_jit knob.
	const passthroughEnv: [string, string][] = [];
	for (const [name, val] of queryOverrides("wj.")) {
		if (/^(MONO|IKVM|DOTNET|WINK|GEX|MESA|GALLIUM|NIR)_/i.test(name))
			passthroughEnv.push([name.toUpperCase(), val]);
		else overrides[name.toLowerCase()] = val;
	}

	const overridden = new Set(
		Object.keys(overrides).map((k) => k.toLowerCase())
	);
	wasm_jit = Object.assign(wasm_jit, overrides);

	for (let [opt, val] of Object.entries(wasm_jit)) {
		if (val == null) continue;
		const name = `MONO_WASM_JIT_${opt.toUpperCase()}`;
		builder.withEnvironmentVariable(name, `${val}`);
		if (overridden.has(opt.toLowerCase()))
			console.log(`[harness] env override: ${name}=${val}`);
	}
	for (const [name, val] of passthroughEnv) {
		builder.withEnvironmentVariable(name, val);
		console.log(`[harness] env override: ${name}=${val}`);
	}

	runtime = await builder.create();

	config = runtime.getConfig();
	exports = await runtime.getAssemblyExports(config.mainAssemblyName!);

	let Module = runtime.Module as any;

	Module.canvas = canvas;
	Module.glfwOnWindowCreated = (window: any) => {
		const initialFrame = Module._emscripten_glfw3_get_frame_count();
		const timer = setInterval(() => {
			if (Module._emscripten_glfw3_get_frame_count() <= initialFrame) return;

			clearInterval(timer);
			Module.glfwMakeCanvasResizable(window, ".canvas");
		}, 50);
	};

	(globalThis as any).wasm = {
		Module,
		FS: Module.FS,
		dotnet,
		runtime,
		config,
		exports,
		canvas,
	};
	(globalThis as any).captureJitSlotBaseline?.();

	console.debug("PreInit...");
	await runtime.runMain();
	const fetchbase = location.origin + location.pathname;
	const sysProps: [string, string][] = [
		["org.lwjgl.util.Debug", "true"],
		["org.lwjgl.util.DebugLoader", "true"],
		["java.awt.headless", "true"],
		["sodium.checks.issue2561", "false"],
	];
	// ?jprop.<name>=<value>: extra Java system properties, e.g. the jbox2d bench's schedule (bench.warmup /
	// bench.batchSteps / bench.batches, read by IkvmBenchmarkStarter; scratchpad/wj/j2d/run.mjs --schedule).
	for (const [k, v] of new URLSearchParams(location.search))
		if (k.startsWith("jprop.")) sysProps.push([k.slice(6), v]);

	await exports.IkvmWasm.PreInit(
		fetchbase,
		["/libsdl/bench.jar"],
		getDlls().map((x) => `${x[0]}|${x[1]}`),
		sysProps
	);
	console.debug("dotnet initialized");
	console.timeEnd("dotnet ");
}

export async function play(version: string, authInfo: MinecraftAuthInfo) {
	let mod = (globalThis as any).wasm.Module;
	// O(1) counters (loader/Emscripten.c): native = malloc in use minus the GC's blocks, gc heap = those blocks
	// (shown after the committed number below), heap = the sbrk break (what hits the 4 GiB; the wasm length grows
	// ahead of it)
	setInterval(() => {
		dotnetState.wasmMemory = mod.wasmMemory.buffer.byteLength / (1024 * 1024);
		dotnetState.nativeMemory = mod._ikvm_native_used_mb();
		dotnetState.gcHeapMemory = mod._ikvm_gc_heap_mb();
		dotnetState.heapMemory = mod._ikvm_heap_top_mb();
	}, 5000);

	exports.IkvmWasm.WatchMemoryUsage(5000, (live: number) => {
		dotnetState.gcMemory = live;
		return false;
	});

	let framecounts = [];
	let lastframecount = 0;
	setInterval(() => {
		let framecount = (
			globalThis as any
		).wasm.Module._emscripten_glfw3_get_frame_count();
		dotnetState.fps = framecount - lastframecount;
		framecounts.push(dotnetState.fps);
		lastframecount = framecount;

		if (framecounts.length > 10) framecounts.shift();

		dotnetState.fpsAvg =
			framecounts.reduce((acc, x) => acc + x, 0) / framecounts.length;
	}, 1000);

	if (new URLSearchParams(location.search).has("bench")) {
		console.debug("Run bench.jar...");
		await exports.IkvmWasm.RunJar("/libsdl/bench.jar");
	} else {
		console.debug("Run...");
		await exports.IkvmWasm.Run(version, authInfo.playerName, authInfo.uuid, authInfo.accessToken, authInfo.userType);
	}
	console.debug("Exited");
}
