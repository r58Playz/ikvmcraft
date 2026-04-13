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

console.log = new Proxy(console.log, {
	apply(target, thisArg, argArray) {
	    dotnetState.logs = [...dotnetState.logs, argArray.join(" ")];
		return Reflect.apply(target, thisArg, argArray);
	},
})

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
		/*
		.withEnvironmentVariable("MONO_LOG_LEVEL", "debug")
		.withEnvironmentVariable("MONO_LOG_MASK", "all")
		.withEnvironmentVariable("IKVM_DISABLE_STACKTRACE_CLEANING", "true")
		*/
		.withRuntimeOptions([
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
	await exports.IkvmWasm.PreInit(location.href, [["org.lwjgl.util.Debug", "true"], ["renderDistanceChunks", "2"]]);
	console.debug("dotnet initialized");
	console.timeEnd("dotnet ");
}

export async function play() {
	console.debug("Run...");
	await exports.IkvmWasm.Run("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.game.VoxelGameGL");
	//await exports.IkvmWasm.Run("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.camera.FreeCameraDemo");
	//await exports.IkvmWasm.Run("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.shadow.ShadowMappingDemo20");
	console.debug("Exited");
}
