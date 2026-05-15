import { createState } from "dreamland/core";
import type { ModuleAPI, MonoConfig, RuntimeAPI } from "./dotnetdefs";

const wasm: ModuleAPI = await eval(`import("/_framework/dotnet.js")`);
const dotnet = wasm.dotnet;
let runtime: RuntimeAPI;
let config: MonoConfig;
let exports: any;

export let dotnetState = createState({
	logs: [] as string[]
});

/*
console.log = new Proxy(console.log, {
	apply(target, thisArg, argArray) {
		dotnetState.logs = [...dotnetState.logs, argArray.join(" ")];
		return Reflect.apply(target, thisArg, argArray);
	},
})
(globalThis as any).logs = []
console.log = new Proxy(console.log, {
	apply(target, thisArg, argArray) {
		(globalThis as any).logs.push(argArray);
	},
})
*/

const rootFolder = await navigator.storage.getDirectory();
(globalThis as any).selectjar = async () => {
	let [file] = await showOpenFilePicker();
	const data = await file.getFile().then((r) => r.stream());
	let handle = await rootFolder.getFileHandle("main.jar", { create: true });
	const writable = await handle.createWritable();
	await data.pipeTo(writable);
}

export async function initDotnet(canvas: HTMLCanvasElement) {
	console.time("dotnet ");
	runtime = await dotnet
		.withConfig({ pthreadPoolInitialSize: 16 })
		.withEnvironmentVariable("MONO_SLEEP_ABORT_LIMIT", "20000")
		//.withEnvironmentVariable("MONO_LOG_LEVEL", "debug")
		//.withEnvironmentVariable("MONO_LOG_MASK", "type")
		//.withEnvironmentVariable("IKVM_FROMCLASS_TRACE", "1")
		//.withEnvironmentVariable("IKVM_UNSAFE_OFFSET_TRACE", "1")
		.withRuntimeOptions([
			
			/*
			// jit functions quickly and jit more functions
			`--jiterpreter-minimum-trace-hit-count=${500}`,

			// monitor jitted functions for less time
			`--jiterpreter-trace-monitoring-period=${100}`,

			// reject less funcs
			`--jiterpreter-trace-monitoring-max-average-penalty=${150}`,

			// increase jit function limits
			`--jiterpreter-wasm-bytes-limit=${64 * 1024 * 1024}`,
			`--jiterpreter-table-size=${32 * 1024}`,

			// print jit stats
			`--jiterpreter-stats-enabled`,
			*/
			
			
			`--no-jiterpreter-traces-enabled`
		])
		.create();

	// why??
	let pump = (runtime.Module as any).wasmExports["emscripten_main_thread_process_queued_calls"];
	setInterval(() => {
		pump();
	}, 1);

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
	await exports.IkvmWasm.PreInit(location.href, [["org.lwjgl.util.Debug", "true"], ["org.lwjgl.util.DebugLoader", "true"]]);
	console.debug("dotnet initialized");
	console.timeEnd("dotnet ");
}

export async function play() {
	console.debug("Run...");
	//await triageSetupAndInvoke("org.lwjgl.system.MemoryUtil");
	//await reflectInvoke("org.lwjgl.system.Pointer$Default");
	//await reflectInvoke("org.lwjgl.system.SharedLibrary$Default");
	//await reflectInvoke("org.lwjgl.system.Callback");
	await exports.IkvmWasm.Run()
	//await exports.IkvmWasm.RunJar("/assets/log4j-demo.jar")
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.game.VoxelGameGL");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.camera.FreeCameraDemo");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.shadow.ShadowMappingDemo20");
	console.debug("Exited");
}

/**
 * Triage helper: set up Minecraft classpath without launching, then drive
 * specific Java classes by reflection. Use to bisect which class triggers the
 * bug. Call from devtools as `wasm.triage("net.minecraft.Util")` etc.
 */
export async function reflectInvoke(className: string, methodName: string = "", args: string[] = []) {
	console.debug(`[triage] reflect-invoke ${className}.${methodName || "<load>"}(${args.join(", ")})`);
	await exports.IkvmWasm.ReflectInvoke(className, methodName, args);
}

export async function triageSetupAndInvoke(className: string, methodName: string = "", args: string[] = []) {
	console.debug("[triage] setting up minecraft classpath...");
	await exports.IkvmWasm.SetupMinecraft();
	await reflectInvoke(className, methodName, args);
}

(globalThis as any).triage = triageSetupAndInvoke;
(globalThis as any).triageInvoke = reflectInvoke;

/**
 * Run the full Minecraft Main.main(String[]) via the triage path. Use this
 * to test whether the bug reproduces when only the launcher main code runs,
 * with no extra setup beyond what SetupMinecraft does.
 */
export async function triageMain() {
	console.debug("[triage] setting up + running Main.main...");
	await exports.IkvmWasm.SetupMinecraft();
	await exports.IkvmWasm.RunMinecraftMain();
}
(globalThis as any).triageMain = triageMain;
