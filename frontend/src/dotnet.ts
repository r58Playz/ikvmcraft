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
		.withEnvironmentVariable("MONO_GC_PARAMS", "nursery-size=128m")
		//.withEnvironmentVariable("IKVM_FROMCLASS_TRACE", "1")
		//.withEnvironmentVariable("IKVM_UNSAFE_OFFSET_TRACE", "1")
		.withRuntimeOptions([
			
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
			`--jiterpreter-table-size=${32 * 1024}`,
			`--jiterpreter-aot-table-size=${32 * 1024}`,

			// print jit stats
			//`--jiterpreter-stats-enabled`,
			//`--jiterpreter-count-bailouts`,
			//`--jiterpreter-estimate-heat`,

			//`--jiterpreter-direct-jit-calls=false`,

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

// Interp PGO profile: mojmap class -> [mojmap method name, mojmap JVM descriptor][].
// These are the hottest *interpreter* methods from gameplay heat dumps
export type PgoProfile = Record<string, [string, string][]>;

let pgoProfile: PgoProfile = {
	// texture upload / pixel access. checkAllocated is the #1 hottest trace (98M hits, never jitted)
	// because its tier-0 callers (getPixelRGBA/setPixelRGBA/_upload) never inline it.
	"com.mojang.blaze3d.platform.NativeImage": [
		["checkAllocated", "()V"],
		["getPixelRGBA", "(II)I"],                 // method_4315, 65M
		["setPixelRGBA", "(III)V"],                // method_4305, 32M
		["getLuminanceOrAlpha", "(II)B"],
		["makePixelArray", "()[I"],
		["_upload", "(IIIIIIIZZZZ)V"],
	],
	// #2 hottest (70M) — guards nearly every GL call on the render thread.
	"com.mojang.blaze3d.systems.RenderSystem": [
		["assertThread", "(Ljava/util/function/Supplier;)V"],
		["recordRenderCall", "(Lcom/mojang/blaze3d/pipeline/RenderCall;)V"],
	],
	// block/fluid lookups — the chunk-access hot path (getBlockState 58M + 44M + 15M).
	"net.minecraft.world.level.Level": [
		["getBlockState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/block/state/BlockState;"],
		["getFluidState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/material/FluidState;"],
		// lithium mixin-added method on Level — not in mojmap, matched by literal name (empty descriptor
		// => by-name; it's unique). 56M, the biggest single unmapped hitter.
		["getChunkLithium", ""],
	],
	"net.minecraft.world.level.chunk.LevelChunk": [
		["getBlockState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/block/state/BlockState;"],
		["getFluidState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/material/FluidState;"],
	],
	"net.minecraft.world.level.chunk.ProtoChunk": [
		["getBlockState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/block/state/BlockState;"],
		["getFluidState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/material/FluidState;"],
	],
	// the section/palette array access reached through getBlockState.
	"net.minecraft.world.level.chunk.LevelChunkSection": [
		["getBlockState", "(III)Lnet/minecraft/world/level/block/state/BlockState;"],
		["getFluidState", "(III)Lnet/minecraft/world/level/material/FluidState;"],
		["isRandomlyTicking", "()Z"],   // class_2826.method_12262, 10M
	],
	"net.minecraft.world.level.chunk.PalettedContainer": [
		["get", "(III)Ljava/lang/Object;"],
	],
	// #7 hottest (38M) — entity data tracker reads, called per-entity per-tick.
	"net.minecraft.network.syncher.SynchedEntityData": [
		["getItem", "(Lnet/minecraft/network/syncher/EntityDataAccessor;)Lnet/minecraft/network/syncher/SynchedEntityData$DataItem;"],
	],
	// face-culling during chunk meshing (13M).
	"net.minecraft.world.level.block.Block": [
		["shouldRenderFace", "(Lnet/minecraft/world/level/block/state/BlockState;Lnet/minecraft/world/level/BlockGetter;Lnet/minecraft/core/BlockPos;Lnet/minecraft/core/Direction;)Z"],
	],
	// coordinate accessors — inlined into every block lookup / index computation.
	"net.minecraft.core.Vec3i": [
		["getX", "()I"],
		["getY", "()I"],
		["getZ", "()I"],
	],
	"net.minecraft.core.BlockPos": [
		["relative", "(Lnet/minecraft/core/Direction;)Lnet/minecraft/core/BlockPos;"],   // method_10093, 14M
		["above", "()Lnet/minecraft/core/BlockPos;"],
		["asLong", "()J"],
		["asLong", "(III)J"],     // static pack
		["getX", "(J)I"],         // static unpack
		["getY", "(J)I"],
		["getZ", "(J)I"],
	],
	"net.minecraft.core.BlockPos$MutableBlockPos": [
		["move", "(Lnet/minecraft/core/Direction;I)Lnet/minecraft/core/BlockPos$MutableBlockPos;"],          // method_10104, 10M
		["setWithOffset", "(Lnet/minecraft/core/Vec3i;III)Lnet/minecraft/core/BlockPos$MutableBlockPos;"],   // method_25504, 7.4M
	],
	// stack comparison in inventory / entity ticking (8M).
	"net.minecraft.world.item.ItemStack": [
		["matches", "(Lnet/minecraft/world/item/ItemStack;Lnet/minecraft/world/item/ItemStack;)Z"],
	],
	// per-vertex build path. BufferVertexConsumer provides the default impls for these (the heat dump
	// attributes them to class_4584/<default>); they're declared on the VertexConsumer interface in the
	// intermediary mapping, so the name-map misses and they resolve via the signature fallback. These
	// surfaced as the top non-RenderSystem hitters once the texture/block cost dropped (vertex 29M,
	// color 28M). Each returns VertexConsumer (for chaining); distinct param sigs keep matching exact.
	"com.mojang.blaze3d.vertex.BufferVertexConsumer": [
		["vertex",  "(DDD)Lcom/mojang/blaze3d/vertex/VertexConsumer;"],   // method_22912, 29M
		["color",   "(IIII)Lcom/mojang/blaze3d/vertex/VertexConsumer;"],  // method_1336, 28M
		["normal",  "(FFF)Lcom/mojang/blaze3d/vertex/VertexConsumer;"],
		["uv",      "(FF)Lcom/mojang/blaze3d/vertex/VertexConsumer;"],    // method_22913
		["uvShort", "(SSI)Lcom/mojang/blaze3d/vertex/VertexConsumer;"],   // method_22899
	],
	// GL vertex-state teardown, per draw (class_293.method_22651, 6M).
	"com.mojang.blaze3d.vertex.VertexFormat": [
		["clearBufferState", "()V"],
	],
	// face-occlusion test during chunk meshing (class_259.method_20713, ~3M).
	"net.minecraft.world.phys.shapes.Shapes": [
		["faceShapeOccludes", "(Lnet/minecraft/world/phys/shapes/VoxelShape;Lnet/minecraft/world/phys/shapes/VoxelShape;)Z"],
	],
	// model quads during chunk meshing (class_1097.method_4707, 15M).
	"net.minecraft.client.resources.model.WeightedBakedModel": [
		["getQuads", "(Lnet/minecraft/world/level/block/state/BlockState;Lnet/minecraft/core/Direction;Ljava/util/Random;)Ljava/util/List;"],
	],
	// per-block render-layer lookup during meshing (class_4696.method_23679, 10M).
	"net.minecraft.client.renderer.ItemBlockRenderTypes": [
		["getChunkRenderType", "(Lnet/minecraft/world/level/block/state/BlockState;)Lnet/minecraft/client/renderer/RenderType;"],
	],
	// the render region's block/fluid access — getBlockState/getFluidState are BlockGetter-declared,
	// so they resolve via the signature fallback (class_1950.method_8320, 8M).
	"net.minecraft.world.level.PathNavigationRegion": [
		["getBlockState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/block/state/BlockState;"],
		["getFluidState", "(Lnet/minecraft/core/BlockPos;)Lnet/minecraft/world/level/material/FluidState;"],
	],
	// light lookup during meshing (class_4538.method_22346, default on LevelReader, ~5M).
	"net.minecraft.world.level.LevelReader": [
		["getMaxLocalRawBrightness", "(Lnet/minecraft/core/BlockPos;I)I"],
	],
};

export async function play(version: string) {
	let mappedProfile = Object.entries(pgoProfile).map(([klass, funcs]) => [klass, ...funcs.map(([name, desc]) => `${name}|${desc}`)]);

	console.debug("Run...");
	await exports.IkvmWasm.Run(version, mappedProfile);
	//await exports.IkvmWasm.RunJar("/assets/log4j-demo.jar")
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.game.VoxelGameGL");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.camera.FreeCameraDemo");
	//await exports.IkvmWasm.RunJar("/assets/lwjgl3-demos.jar", "org.lwjgl.demo.opengl.shadow.ShadowMappingDemo20");
	console.debug("Exited");
}

/**
 * Dump the hottest jiterpreter trace entry points
 * A hot method showing jit<eps (few/no compiled entry points) is one the jiterpreter couldn't
 * trace — i.e. an interpreter-time / inlining target.
 */
export function dumpJiterpHeat(top = 60): void {
	(globalThis as any).wasm?.Module._dump_jiterp_trace_heat(top);
	(globalThis as any).wasm.runtime.INTERNAL.jiterpreter_dump_stats(false);
}
(globalThis as any).dumpJiterpHeat = dumpJiterpHeat;
