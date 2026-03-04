import type { ModuleAPI, MonoConfig, RuntimeAPI } from "./dotnetdefs";

const wasm: ModuleAPI = await eval(`import("/_framework/dotnet.js")`);
const dotnet = wasm.dotnet;
let runtime: RuntimeAPI;
let config: MonoConfig;
let exports: any;

export async function initDotnet(canvas: HTMLCanvasElement) {
	console.time("dotnet ");
	runtime = await dotnet.withEnvironmentVariables({ "FONTCONFIG_PATH": "/etc/fonts" }).create();

	config = runtime.getConfig();
	exports = await runtime.getAssemblyExports(config.mainAssemblyName!);
	(runtime.Module as any).canvas = canvas;

	Object.assign(globalThis, {
		Module: runtime.Module,
		FS: (runtime.Module as any).FS,
		dotnet,
		runtime,
		config,
		exports,
		canvas,
	});
	console.debug("PreInit...");
	await runtime.runMain();
	await exports.IkvmWasm.PreInit(location.href);
	console.debug("dotnet initialized");
	console.timeEnd("dotnet ");
}

export async function play() {
	console.debug("Run...");
	await exports.IkvmWasm.Run();
	console.debug("Exited");
}
