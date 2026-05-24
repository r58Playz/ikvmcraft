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
*/
/*
(globalThis as any).logs = []
console.debug = new Proxy(console.debug, {
	apply(target, thisArg, argArray) {
		(globalThis as any).logs.push(argArray);
	},
})
*/

let hackfixtimer = -1;
export async function initDotnet(canvas: HTMLCanvasElement) {
	console.time("dotnet ");
	//(globalThis as any).GLFW3_DEBUG = true;
	runtime = await dotnet
		.withConfig({ pthreadPoolInitialSize: 16 })
		.withModuleConfig({
			onRuntimeInitialized(Module: any) {
				(globalThis as any).wasm = { Module, FS: Module.FS };
				hackfixtimer = setInterval(() => Module.checkMailbox(), 4);
			}
		})
		.withEnvironmentVariable("MONO_SLEEP_ABORT_LIMIT", "20000")
		//.withEnvironmentVariable("MONO_LOG_LEVEL", "debug")
		//.withEnvironmentVariable("MONO_LOG_MASK", "aot")
		//.withEnvironmentVariable("IKVM_FROMCLASS_TRACE", "1")
		//.withEnvironmentVariable("IKVM_UNSAFE_OFFSET_TRACE", "1")
		.withRuntimeOptions([
			// accept smaller traces earlier
			`--jiterpreter-minimum-trace-value=${10}`,
			`--jiterpreter-minimum-trace-hit-count=${1000}`,
			`--jiterpreter-minimum-distance-between-traces=${3}`,

			// increase jit function limits
			`--jiterpreter-wasm-bytes-limit=${64 * 1024 * 1024}`,
			`--jiterpreter-max-module-size=${24 * 1024 - 1}`,
			`--jiterpreter-table-size=${32 * 1024}`,

			// print jit stats
			`--jiterpreter-stats-enabled`,

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
	await exports.IkvmWasm.PreInit(location.href, [["org.lwjgl.util.Debug", "true"], ["org.lwjgl.util.DebugLoader", "true"], ["java.awt.headless", "true"]]);
	console.debug("dotnet initialized");
	console.timeEnd("dotnet ");
	clearInterval(hackfixtimer);
}

export async function play() {
	console.debug("Run...");
	await exports.IkvmWasm.Run()
	//await exports.IkvmWasm.RunJar("/assets/log4j-demo.jar")
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.game.VoxelGameGL");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.camera.FreeCameraDemo");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.shadow.ShadowMappingDemo20");
	console.debug("Exited");
}
