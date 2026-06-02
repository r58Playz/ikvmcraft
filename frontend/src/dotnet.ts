import type { ModuleAPI, MonoConfig, RuntimeAPI } from "./dotnetdefs";

function crashMinecraftF3C(holdMs = 10500) {
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

export async function play(version: string) {
	console.debug("Run...");
	await exports.IkvmWasm.Run(version)
	//await exports.IkvmWasm.RunJar("/assets/log4j-demo.jar")
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.game.VoxelGameGL");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.camera.FreeCameraDemo");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.shadow.ShadowMappingDemo20");
	console.debug("Exited");
}
