import type { ModuleAPI, MonoConfig, RuntimeAPI } from "./dotnetdefs";
import { EpxTcpWs } from "./net";

export function crashMinecraftF3C(holdMs = 10500) {
	const send = (type: string, code: string, key: string, repeat: boolean) =>
		document.dispatchEvent(new KeyboardEvent(type, {
			code, key, repeat, bubbles: true, cancelable: true,
		}));
	send('keydown', 'F3',   'F3', false);
	send('keydown', 'KeyC', 'c',  false);
	const tick = setInterval(() => {
		send('keydown', 'F3',   'F3', true);
		send('keydown', 'KeyC', 'c',  true);
	}, 200);
	setTimeout(() => {
		clearInterval(tick);
		send('keyup', 'KeyC', 'c',  false);
		send('keyup', 'F3',   'F3', false);
	}, holdMs);

	console.log(`[f3c] holding F3+C for ${holdMs}ms`);
}
(globalThis as any).crashMinecraftF3C = crashMinecraftF3C;

/**
 * Prompts the user to pick a file and uploads it to OPFS at the given path.
 * Supports nested paths (e.g. "folder/subfolder/file.txt").
 *
 * @param filePath - The destination path within OPFS (e.g. "uploads/photo.png")
 * @returns The FileSystemFileHandle of the written file
 */
async function upload(filePath: string): Promise<FileSystemFileHandle> {
  // Prompt the user to pick a file
  const [pickedHandle] = await (window as any).showOpenFilePicker();
  const file = await pickedHandle.getFile();
 
  // Navigate / create directories for any nested path segments
  const segments = filePath.split("/").filter(Boolean);
  const fileName = segments.pop();
  if (!fileName) throw new Error("filePath must include a file name");
 
  let dir: FileSystemDirectoryHandle = await navigator.storage.getDirectory();
  for (const segment of segments) {
    dir = await dir.getDirectoryHandle(segment, { create: true });
  }
 
  // Create (or replace) the file at the destination
  const destHandle = await dir.getFileHandle(fileName, { create: true });
  const writable = await destHandle.createWritable();
  await writable.write(file);
  await writable.close();
 
  return destHandle;
}
(globalThis as any).upload = upload;

const wasm: ModuleAPI = await eval(`import("/_framework/dotnet.js")`);
const dotnet = wasm.dotnet;
let runtime: RuntimeAPI;
let config: MonoConfig;
let exports: any;

export type Log = { color: string; log: string };
export let loglisteners: ((log: Log) => void)[] = [];

let logs: string[] = [];
(globalThis as any).logs = logs;

function proxyConsole(name: string, color: string) {
	// @ts-expect-error ts sucks
	const old = console[name].bind(console);
	// @ts-expect-error ts sucks
	console[name] = (...args) => {
		let str;
		try {
			str = args.join(" ");
		} catch {
			str = "<failed to render>";
		}
		old(...args);
		for (const logger of loglisteners) {
			logger({ color, log: str });
		}
		logs.push(str);
	};
	return old;
}
export const bypassError = proxyConsole("error", "var(--error)");
export const bypassWarn = proxyConsole("warn", "var(--warning)");
export const bypassLog = proxyConsole("log", "var(--fg)");
export const bypassInfo = proxyConsole("info", "var(--info)");
export const bypassDebug = proxyConsole("debug", "var(--fg4)");
(globalThis as any).bypassLog = bypassLog;

export function getDlls(): (readonly [string, string])[] {
	const config = (wasm.dotnet as any).instance.config;
	const resources = [
		...(config.resources?.coreAssembly || []),
		...(config.resources?.assembly || []),
	];
	return resources.map((x) => [x.name, x.virtualPath] as const);
}

export async function initDotnet(canvas: HTMLCanvasElement) {
	// emscripten proxy hackfix number 39847232303
	(globalThis as any).Atomics.waitAsync = undefined;

	(globalThis as any).WebSocket = new Proxy(WebSocket, {
		construct(t, a, n) {
			const url = new URL(a[0]);
			if (url.hostname.startsWith("wisp-"))
				return new EpxTcpWs(url);

			return Reflect.construct(t, a, n);
		},
	});

	console.time("dotnet ");
	//(globalThis as any).GLFW3_DEBUG = true;
	runtime = await dotnet
		.withConfig({ pthreadPoolInitialSize: 16 })
		.withModuleConfig({
			onRuntimeInitialized(Module: any) {
				(globalThis as any).wasm = { Module, FS: Module.FS };
			}
		})
		.withEnvironmentVariable("MONO_SLEEP_ABORT_LIMIT", "20000")
		//.withEnvironmentVariable("MONO_LOG_LEVEL", "debug")
		//.withEnvironmentVariable("MONO_LOG_MASK", "gc")
		//.withEnvironmentVariable("MONO_LOG_MASK", "aot")
		.withEnvironmentVariable("MONO_GC_PARAMS", "nursery-size=16m")
		.withEnvironmentVariable("DOTNET_DiagnosticPorts", "js://ondemand,nosuspend")
		.withEnvironmentVariable("DOTNET_WasmPerformanceInstrumentation", "eventpipe,callspec=all")
		//.withEnvironmentVariable("IKVM_FROMCLASS_TRACE", "1")
		//.withEnvironmentVariable("IKVM_UNSAFE_OFFSET_TRACE", "1")
		.withRuntimeOptions([
			
			// accept smaller traces earlier
			`--jiterpreter-minimum-trace-value=${10}`,
			`--jiterpreter-minimum-trace-hit-count=${1000}`,
			`--jiterpreter-back-branch-boost=${980}`, // make sure this is below trace hit count
			`--jiterpreter-minimum-distance-between-traces=${3}`,
			`--jiterpreter-trace-monitoring-period=${500}`,
			`--jiterpreter-trace-monitoring-max-average-penalty=${50}`,

			// increase jit function limits
			`--jiterpreter-wasm-bytes-limit=${64 * 1024 * 1024}`,
			`--jiterpreter-max-module-size=${64 * 1024 - 1}`,
			`--jiterpreter-table-size=${32 * 1024}`,

			// print jit stats
			`--jiterpreter-stats-enabled`,
			

			//`--no-jiterpreter-jit-call-enabled`,
			//`--no-jiterpreter-interp-entry-enabled`,

			//`--no-jiterpreter-traces-enabled`
		])
		.create();


	config = runtime.getConfig();
	exports = await runtime.getAssemblyExports(config.mainAssemblyName!);


	(runtime.Module as any).canvas = canvas;

	(globalThis as any).wasm = {
		Module: runtime.Module,
		FS: (runtime.Module as any).FS,
		dotnet,
		runtime,
		config,
		exports,
		canvas,
	};
	console.debug("PreInit...");
	await runtime.runMain();
	await exports.IkvmWasm.PreInit(location.href, getDlls().map((x) => `${x[0]}|${x[1]}`), [["org.lwjgl.util.Debug", "true"], ["org.lwjgl.util.DebugLoader", "true"], ["java.awt.headless", "true"]]);
	console.debug("dotnet initialized");
	console.timeEnd("dotnet ");
}

/**
 * Collect an EventPipe CPU-sampling .nettrace, on demand, from the main/UI thread.
 *
 * The EventPipe diagnostic server (and its JS `serverSession`) live on a worker thread, so
 * collection runs there. We bridge across threads through a shared-memory control block
 * exposed by the app's Emscripten.c (`diag_trace_*`): the UI thread posts a request, the DS
 * worker services it from its own poll loop and `_malloc`s + publishes the bytes, and we read
 * them back out of `HEAPU8` here. Our harness (not the runtime) then writes OPFS, so the
 * trace survives page exit.
 *
 * Requires a profiler-enabled build (IkvmWasmEnableProfiler=true) on the patched runtime.
 * View with: PerfView, Visual Studio, `dotnet-trace convert --format speedscope`,
 * or https://ui.perfetto.dev (after converting).
 */
export async function collectTrace(durationSeconds = 10): Promise<number> {
	const Module: any = (globalThis as any).wasm?.Module;
	if (!Module || typeof Module._diag_trace_request !== "function" || typeof Module._diag_stream_is_done !== "function")
		throw new Error("diag trace shims unavailable — need a profiler build (IkvmWasmEnableProfiler=true) on the patched runtime");

	// clear the cross-worker .nettrace accumulator + done flag from any previous run
	Module._diag_stream_reset();

	console.debug(`[trace] requesting ${durationSeconds}s CPU trace from the DS worker...`);
	Module._diag_trace_request(Math.round(durationSeconds * 1000));

	// Wait for the session to finish. The cross-worker connection close isn't observed on the
	// main thread (the streaming worker has no socket_handles), so diag_stream_is_done may never
	// flip; instead treat the stream as complete once it has PLATEAUED (no growth for a stable
	// window) after the requested collection duration. The post-stop rundown (all loaded
	// methods/assemblies — large for IKVM) streams slowly, so allow a generous hard timeout.
	const start = performance.now();
	const stableMs = 3000;                              // no growth for this long => done
	const hardTimeoutMs = durationSeconds * 1000 + 240000;
	let lastLen = -1;
	let lastChange = performance.now();
	for (;;) {
		const now = performance.now();
		if (Module._diag_stream_is_done()) break;       // clean close, if it ever happens
		const cur = Module._diag_stream_len() | 0;
		if (cur !== lastLen) { lastLen = cur; lastChange = now; }
		else if (cur > 0 && now - start > durationSeconds * 1000 && now - lastChange > stableMs) break;
		if (now - start > hardTimeoutMs)
			throw new Error("[trace] timed out (stream never plateaued; len=" + lastLen + ")");
		await new Promise((r) => setTimeout(r, 250));
	}

	const ptr = Module._diag_stream_ptr() >>> 0;
	const len = Module._diag_stream_len() | 0;
	if (!ptr || len <= 0) {
		Module._diag_stream_reset();
		throw new Error("[trace] session closed but no .nettrace bytes were captured (see console)");
	}
	const bytes: Uint8Array = Module.HEAPU8.slice(ptr, ptr + len);
	Module._diag_stream_reset();

	// persist to OPFS from our harness so it survives page exit
	const name = `trace-${Date.now()}.nettrace`;
	try {
		const root = await navigator.storage.getDirectory();
		const dir = await root.getDirectoryHandle("traces", { create: true });
		const fh = await dir.getFileHandle(name, { create: true });
		const w = await fh.createWritable();
		await w.write(bytes);
		await w.close();
		console.debug(`[trace] wrote ${len} bytes to OPFS /traces/${name}`);
	} catch (e) {
		console.error(`[trace] OPFS write failed: ${e}`);
	}

	(globalThis as any).__lastTrace = bytes;
	console.debug(`[trace] done: ${len} bytes (globalThis.__lastTrace, OPFS /traces/${name})`);
	return len;
}
(globalThis as any).collectTrace = collectTrace;

export async function play(version: string) {
	console.debug("Run...");
	await exports.IkvmWasm.Run(version)
	//await exports.IkvmWasm.RunJar("/assets/log4j-demo.jar")
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.game.VoxelGameGL");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.camera.FreeCameraDemo");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.shadow.ShadowMappingDemo20");
	console.debug("Exited");
}
