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

// --- benchmark input injector -------------------------------------------------------------------
// Drives a deterministic, repeatable movement path so jiterpreter heat / bailout counters and frame
// time can be sampled over a fixed window. emscripten-glfw listens for keydown/keyup on `document`
// and reads e.code (KeyboardMapping.h), so dispatching synthetic KeyboardEvents drives the game
// exactly like real keys. GLFW tracks press/release state (polled via glfwGetKey each frame), so a
// single keydown "presses" a key until we send keyup — no need to spam repeat events.
//
// Mouse-look is deliberately NOT injected: turning needs movementX/Y under pointer lock, which a
// synthetic event can't acquire (pointer lock requires a real user gesture). Walking forward through
// a superflat world is the deterministic render workload we want — new chunks continuously come into
// view ahead and get meshed (the vertex-consumer / chunk-mesh hot path), without camera variance.
//
// Recommended setup before running: a frozen superflat world at a fixed spawn —
//   /gamerule doDaylightCycle false   /time set day
//   /gamerule doWeatherCycle false    /weather clear
//   /gamerule randomTickSpeed 0       /difficulty peaceful
// The button in the UI disables while this runs and re-enables when done: that is your measurement
// window. Snapshot counters (dumpJiterpHeat / bailout totals) when it disables and again when it
// re-enables, diff, and normalize per-frame.

function benchKey(type: "keydown" | "keyup", code: string, key: string) {
	document.dispatchEvent(new KeyboardEvent(type, { code, key, repeat: false, bubbles: true, cancelable: true }));
}
const benchSleep = (ms: number) => new Promise<void>((r) => setTimeout(r, ms));

let benchStop: (() => void) | null = null;
export const benchRunning = () => benchStop !== null;

/**
 * Walk the player on a fixed path for `durationMs` (default 60s): sprint forward the whole time,
 * alternating a strafe right/left every 4s so the path covers terrain in more than one direction
 * while staying deterministic. Resolves when finished or when stopBench() is called. Always releases
 * every key it pressed, even on error/stop.
 */
export async function benchWalk(durationMs = 60000): Promise<void> {
	if (benchStop) { console.warn("[bench] already running"); return; }

	let stopped = false;
	benchStop = () => { stopped = true; };
	const start = performance.now();
	const elapsed = () => performance.now() - start;

	// every key we might press; released unconditionally in finally
	const allKeys: [string, string][] = [["KeyW", "w"], ["ControlLeft", "Control"], ["KeyD", "d"], ["KeyA", "a"]];

	console.log(`[bench] walk start: ${durationMs}ms (sprint forward + alternating strafe)`);
	try {
		benchKey("keydown", "ControlLeft", "Control"); // sprint (1.16 default keybind)
		benchKey("keydown", "KeyW", "w");               // forward, held throughout

		let curStrafe: "KeyD" | "KeyA" | null = null;
		while (!stopped && elapsed() < durationMs) {
			// 4s phases: forward-only, +right, forward-only, +left, repeat
			const phase = Math.floor(elapsed() / 4000) % 4;
			const want: "KeyD" | "KeyA" | null = phase === 1 ? "KeyD" : phase === 3 ? "KeyA" : null;
			if (want !== curStrafe) {
				if (curStrafe) benchKey("keyup", curStrafe, curStrafe === "KeyD" ? "d" : "a");
				if (want) benchKey("keydown", want, want === "KeyD" ? "d" : "a");
				curStrafe = want;
			}
			await benchSleep(200); // poll stop/elapsed every 200ms
		}
	} finally {
		for (const [code, key] of allKeys) benchKey("keyup", code, key);
		benchStop = null;
		console.log(`[bench] walk done after ${Math.round(elapsed())}ms`);
	}
}
(globalThis as any).benchWalk = benchWalk;

/** Stop an in-progress benchWalk/benchMeasure early (resolves its promise within ~200ms). */
export function stopBench() { benchStop?.(); }
(globalThis as any).stopBench = stopBench;

// --- benchmark measurement harness --------------------------------------------------------------
// Passive: YOU walk the test area for the window; this samples it (no input injection). Brackets the
// window with a heat dump (so you get the abort@ method list before & after) and reports frames, fps,
// ms/frame and per-reason bailout deltas (total + per-frame) — the actionable numbers for A/B'ing a
// runtime change like --jiterpreter-call-continue. The button disables for the whole window = "walk now".
//
// Frame count is emscripten-glfw's swapBuffers() (one bump per presented frame) via a shared-memory
// atomic. CAVEAT: if the scene is under the vsync cap (e.g. a light world at 60fps/6rd), ms/frame is
// vsync-bound, not compute-bound — pick a heavy enough scene (real world / higher render distance) to
// drop below 60 so the metric reflects real frame cost. Bailout deltas are only nonzero on a
// --jiterpreter-count-bailouts build (which itself adds overhead), so read frametime from a clean
// build and bailout deltas from a count-bailouts build — two runs.

// BailoutReason values (jiterpreter-enums.ts)
const BENCH_BAILOUTS: [string, number][] = [
	["Branch", 4], ["BackwardBranch", 5], ["Return", 14], ["Call", 15], ["AllocFailed", 17],
];
const benchFrameCount = (): number => {
	const f = (globalThis as any).wasm?.Module?._emscripten_glfw3_get_frame_count;
	return typeof f === "function" ? f() : NaN;
};
const benchBailout = (reason: number): number => {
	const f = (globalThis as any).wasm?.Module?._mono_jiterp_get_trace_bailout_count;
	return typeof f === "function" ? f(reason) : NaN;
};

/**
 * Measure a fixed window while you manually walk the test area. Snapshots heat at start and end, then
 * reports frames / fps / ms/frame and per-reason bailout deltas. Resolves after `durationMs` or when
 * stopBench() is called.
 */
export async function benchMeasure(durationMs = 60000, walk = false): Promise<void> {
	if (benchStop) { console.warn("[bench] already running"); return; }
	let stopped = false;
	benchStop = () => { stopped = true; };

	const f0 = benchFrameCount();
	const b0 = BENCH_BAILOUTS.map(([, r]) => benchBailout(r));
	const t0 = performance.now();

	console.log(`[bench] measure START — ${walk ? "auto-walking (deterministic)" : "walk the test area now"} for ${(durationMs / 1000).toFixed(0)}s. heat snapshot A:`);
	dumpJiterpHeat(60);

	// when walk=true, drive a deterministic path (sprint forward + alternating strafe every 4s) inside
	// the sampling loop so off/on runs traverse identically. all keys released in finally.
	const walkKeys: [string, string][] = [["KeyW", "w"], ["ControlLeft", "Control"], ["KeyD", "d"], ["KeyA", "a"]];
	const end = t0 + durationMs;
	try {
		if (walk) {
			benchKey("keydown", "ControlLeft", "Control"); // sprint (1.16 default)
			benchKey("keydown", "KeyW", "w");               // forward, held throughout
		}
		let curStrafe: "KeyD" | "KeyA" | null = null;
		while (!stopped && performance.now() < end) {
			if (walk) {
				// 4s phases: forward-only, +right, forward-only, +left, repeat
				const phase = Math.floor((performance.now() - t0) / 4000) % 4;
				const want: "KeyD" | "KeyA" | null = phase === 1 ? "KeyD" : phase === 3 ? "KeyA" : null;
				if (want !== curStrafe) {
					if (curStrafe) benchKey("keyup", curStrafe, curStrafe === "KeyD" ? "d" : "a");
					if (want) benchKey("keydown", want, want === "KeyD" ? "d" : "a");
					curStrafe = want;
				}
			}
			await benchSleep(200);
		}
	} finally {
		if (walk) for (const [code, key] of walkKeys) benchKey("keyup", code, key);
	}

	const t1 = performance.now();
	const f1 = benchFrameCount();
	const b1 = BENCH_BAILOUTS.map(([, r]) => benchBailout(r));
	benchStop = null;

	console.log(`[bench] measure END. heat snapshot B:`);
	dumpJiterpHeat(60);

	const wall = t1 - t0;
	const frames = f1 - f0;
	const out: string[] = [];
	out.push(`=== bench summary — ${(wall / 1000).toFixed(1)}s window ===`);
	if (Number.isFinite(frames) && frames > 0)
		out.push(`frames ${frames}   fps ${(frames / (wall / 1000)).toFixed(1)}   ms/frame ${(wall / frames).toFixed(2)}`);
	else
		out.push(`frames: unavailable — rebuild emscripten-glfw with the frame-counter patch (Module._emscripten_glfw3_get_frame_count missing)`);

	if (Number.isFinite(b0[0])) {
		out.push(`bailouts over window (Δtotal / per-frame):`);
		for (let i = 0; i < BENCH_BAILOUTS.length; i++) {
			const d = b1[i] - b0[i];
			const perFrame = frames > 0 ? (d / frames).toFixed(1) : "?";
			out.push(`  ${BENCH_BAILOUTS[i][0].padEnd(16)} ${Math.round(d).toLocaleString().padStart(16)}   ${perFrame}/frame`);
		}
	} else {
		out.push(`bailout deltas: unavailable — run with --jiterpreter-count-bailouts`);
	}

	console.log(out.join("\n"));
}
(globalThis as any).benchMeasure = benchMeasure;

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

			/*
			// print jit stats
			`--jiterpreter-stats-enabled`,
			`--jiterpreter-count-bailouts`,
			`--jiterpreter-estimate-heat`,
			*/

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
// Mainly used to essentially force inline tiny helper methods that otherwise dont get inlined by interp
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
