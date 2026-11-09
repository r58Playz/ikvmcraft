import { ComponentInstance, css, FC, Stateful } from "dreamland/core";
import "./style.css";
import { initDotnet, play, dotnetState, MinecraftAuthInfo } from "./dotnet";
import { dumpWasmJit } from "./dotnet/jitbench";
import {
	downloadFabricMinecraftVersionToOpfs,
	isMinecraftVersionDownloaded,
	type MinecraftDownloadProgress,
} from "./minecraft";
import { OpfsExplorer } from "./fs";
import { Button, Icon } from "./ui/Button";
import { TextField } from "./ui/TextField";
import { LogView } from "./ui/LogView";
import { createStore } from "dreamland/util";
import {
	getProfile,
	minecraftAuth,
	refreshToken,
	startDeviceCodeAuth,
	Token,
} from "./auth";
import iconDns from "@ktibow/iconset-material-symbols/dns-outline";
import iconCode from "@ktibow/iconset-material-symbols/code";
import iconFolderOpen from "@ktibow/iconset-material-symbols/folder-open";
import iconPlayArrow from "@ktibow/iconset-material-symbols/play-arrow";
import iconLogin from "@ktibow/iconset-material-symbols/login";
import iconLogout from "@ktibow/iconset-material-symbols/logout";

function Logo(this: FC) {
	return <span>ikvmcraft</span>;
}
Logo.style = css`
	:scope {
		font-family: var(--font-display);
		font-size: 2rem;
	}
`;

function Runner(
	this: FC<
		{
			version: string;
			fabric: string;
			auth: MinecraftAuthInfo;
		},
		{
			canvas: HTMLCanvasElement;
			fsOpen: boolean;
			loadState: string;
			downloadProgress: MinecraftDownloadProgress | null;
			loadError: string;
		}
	>
) {
	this.fsOpen = false;
	this.loadState = "Initializing runtime";
	this.downloadProgress = null;
	this.loadError = "";

	let version = this.version;
	let fabric = this.fabric;
	let runtimeReady = false;
	let starting = false;
	const phaseLabels: Record<MinecraftDownloadProgress["phase"], string> = {
		metadata: "Fetching metadata",
		libraries: "Downloading libraries",
		client: "Downloading client",
		mappings: "Downloading mappings",
		assets: "Downloading assets",
		fabric: "Downloading Fabric",
		complete: "Preparing game",
	};
	const start = async () => {
		if (starting) return;
		starting = true;
		this.loadError = "";
		this.downloadProgress = null;
		try {
			if (!runtimeReady) {
				this.loadState = "Initializing runtime";
				await initDotnet(this.canvas);
				runtimeReady = true;
			}
			this.loadState = "Verifying assets";

			if (new URLSearchParams(location.search).has("bench"))
				console.debug("?bench: skipping the Minecraft download check");
			else if (
				!(await isMinecraftVersionDownloaded(`${version}-fabric-${fabric}`, {
					verification: "size",
				}))
			) {
				await downloadFabricMinecraftVersionToOpfs(version, {
					loaderVersion: fabric,
					onProgress: (progress) => {
						this.downloadProgress = progress;
						this.loadState = phaseLabels[progress.phase];
					},
				});
			}

			this.downloadProgress = null;
			this.loadState = "";
			await play(`${version}-fabric-${fabric}`, this.auth);
		} catch (error) {
			console.error("Minecraft launch failed", error);
			this.loadState = "Launch failed";
			this.loadError = error instanceof Error ? error.message : String(error);
			this.downloadProgress = null;
		} finally {
			starting = false;
		}
	};
	this.cx.mount = start;

	const progressFraction = (
		progress: MinecraftDownloadProgress | null
	): number | undefined => {
		if (!progress || progress.totalFiles === 0) return undefined;
		if (progress.completedFiles === progress.totalFiles) return 1;
		return progress.totalBytes !== null && progress.totalBytes > 0
			? Math.min(0.99, progress.downloadedBytes / progress.totalBytes)
			: progress.completedFiles / progress.totalFiles;
	};
	const progressText = (progress: MinecraftDownloadProgress | null): string => {
		if (!progress || progress.totalFiles === 0) return "";
		const files = `${progress.completedFiles.toLocaleString()} / ${progress.totalFiles.toLocaleString()} files`;
		const percent = Math.floor((progressFraction(progress) ?? 0) * 100);
		const bytes =
			progress.totalBytes !== null && progress.totalBytes > 0
				? ` · ${(progress.downloadedBytes / 1048576).toFixed(1)} / ${(progress.totalBytes / 1048576).toFixed(1)} MiB`
				: "";
		return `${percent}% · ${files}${bytes}`;
	};

	let toFixed = (x: number) => x.toFixed(0);

	let threadDump = () => {
		(globalThis as any).wasm.Module.ccall("perform_thread_dump");
	};

	return (
		<div>
			<div class="header">
				<Logo />
				<div class="expand" />
				<div
					class="stats"
					class:hidden={use(this.loadState).map((x) => x !== "")}
				>
					<div class="label fps">fps:</div>
					<div class="stat fps">
						<span>
							{use(dotnetState.fps)} (
							{use(dotnetState.fpsAvg).map((x) => x.toFixed(2))})
						</span>
					</div>
					<div class="label memory">memory (mb):</div>
					<div class="stat memory">
						<span>
							gc {use(dotnetState.gcMemory).map(toFixed)}/
							{use(dotnetState.gcHeapMemory).map(toFixed)}
						</span>
						<span>native {use(dotnetState.nativeMemory).map(toFixed)}</span>
						<span>sbrk {use(dotnetState.heapMemory).map(toFixed)}</span>
						<span>wasm {use(dotnetState.wasmMemory).map(toFixed)}</span>
					</div>
				</div>
				<Button type="normal" icon="left" on:click={threadDump}>
					<Icon icon={iconDns} />
					Thread dump
				</Button>
				<Button type="normal" icon="left" on:click={dumpWasmJit}>
					<Icon icon={iconCode} />
					JIT dump
				</Button>
				<Button
					type="primary"
					icon="full"
					on:click={() => (this.fsOpen = !this.fsOpen)}
				>
					<Icon icon={iconFolderOpen} />
				</Button>
			</div>
			<div class="canvas">
				<div
					class="overlay not-ready"
					class:hidden={use(this.loadState).map((x) => x === "")}
				>
					<div class="load-status" role="status">
						{use(this.loadState)}
					</div>
					<div
						class="download-progress"
						class:inactive={use(this.downloadProgress).map(
							(progress) => progress === null
						)}
					>
						{use(this.downloadProgress).map((progress) => {
							const value = progressFraction(progress);
							return value === undefined ? (
								<progress aria-label="Download progress" max={1} />
							) : (
								<progress
									aria-label="Download progress"
									max={1}
									value={value}
								/>
							);
						})}
						<div class="progress-detail">
							{use(this.downloadProgress).map(progressText)}
						</div>
					</div>
					<div
						class="load-error"
						class:inactive={use(this.loadError).map((error) => error === "")}
					>
						<div role="alert">{use(this.loadError)}</div>
						<Button type="primary" on:click={start}>
							Retry
						</Button>
					</div>
				</div>
				<canvas
					id="canvas"
					this={use(this.canvas)}
					on:contextmenu={(e: Event) => e.preventDefault()}
				/>
				<div class="overlay waiting">Waiting for game</div>
			</div>
			<LogView scrolling={true} />
			<OpfsExplorer open={use(this.fsOpen)} />
		</div>
	);
}
Runner.style = css`
	:scope {
		height: 100%;
		display: flex;
		flex-direction: column;

		background: var(--bg-sub);
	}

	.header {
		padding: 1rem;

		display: flex;
		gap: 1rem;
		align-items: center;

		background: var(--bg);
		border-bottom: 0.1rem solid var(--surface4);
	}

	.stats {
		display: grid;
		grid-auto-rows: max-content 1fr;
		grid-auto-columns: max-content 1fr;
		gap: 0.5rem 1rem;
	}
	.stats .label {
		grid-column: 1;
		justify-self: end;
	}
	.stats .stat {
		grid-column: 2;
	}
	.stats .fps {
		grid-row: 1;
	}
	.stats .memory {
		grid-row: 2;
	}

	.stats .stat.memory {
		display: flex;
		gap: 0.5rem;
	}

	.canvas {
		align-self: center;

		width: min(100%, calc(60dvh * 854 / 480));
		height: auto;
		aspect-ratio: 854 / 480;
		flex: none;
		min-width: 0;
		contain: size;

		display: grid;
		grid-template-areas: "canvas";
		isolation: isolate;
	}
	.canvas > * {
		grid-area: canvas;
	}
	.canvas .overlay {
		height: 100%;
		width: 100%;

		display: flex;
		align-items: center;
		justify-content: center;

		background: var(--surface1);
		font-family: var(--font-display);
		font-size: 4rem;
	}
	.canvas canvas {
		width: 100%;
		height: 100%;
		z-index: 0;
	}
	.canvas .not-ready {
		z-index: 1;
		flex-direction: column;
		gap: 1rem;
		padding: 1.5rem;
		text-align: center;
		font-size: clamp(1.25rem, 3vw, 2rem);
	}
	.download-progress {
		width: min(100%, 28rem);
	}
	.download-progress progress {
		display: block;
		width: 100%;
		height: 1rem;
	}
	.progress-detail {
		margin-top: 0.5rem;
		font-size: 0.875rem;
		color: var(--fg4);
		font-variant-numeric: tabular-nums;
	}
	.load-error {
		display: flex;
		flex-direction: column;
		align-items: center;
		gap: 1rem;
		max-width: 100%;
		max-height: 70%;
		overflow: auto;
		font-size: 0.875rem;
		overflow-wrap: anywhere;
	}
	.inactive {
		display: none;
	}
	.canvas .waiting {
		z-index: -1;
	}

	.expand {
		flex: 1;
	}
	.hidden {
		visibility: hidden;
	}
`;

let authStore: Stateful<{
	token: Token | undefined;
}> = createStore(
	{
		token: undefined,
	},
	// Live/SISU tokens belong to a different client ID than the old Entra flow.
	{ ident: "ikvmcraft-auth-sisu-1", autosave: "auto", backing: "localstorage" }
);

type AuthState =
	| { type: "loading" }
	| { type: "ready" }
	| { type: "authing"; url: string; qr: string }
	| {
			type: "done";
			name: string;
			uuid: string;
			token: string;
	  }
	| { type: "error"; error: any };

function Launcher(
	this: FC<
		{
			version: string;
			fabric: string;
			play: (info: MinecraftAuthInfo) => void;
		},
		{
			fsOpen: boolean;
			authState: AuthState;
		}
	>
) {
	let ver = this.version,
		fab = this.fabric;

	this.fsOpen = false;
	this.authState = { type: "loading" } as AuthState;

	let readyToPlay = use(this.authState).map(
		(x) => import.meta.env.DEV || x.type === "done"
	);

	let populateToken = async (token: Token) => {
		try {
			let mc = await minecraftAuth(token.access);
			let info = await getProfile(mc);

			this.authState = {
				type: "done",
				name: info.name,
				uuid: info.id,
				token: mc,
			};
		} catch (err) {
			this.authState = { type: "error", error: err };
		}
	};

	this.cx.mount = async () => {
		let token: Token | undefined;
		if (!(token = authStore.token)) {
			this.authState = { type: "ready" };
			return;
		}

		if (token.expires < Date.now()) {
			try {
				authStore.token = token = await refreshToken(token.refresh);
			} catch (e) {
				this.authState = { type: "error", error: e };
				return;
			}
		}

		await populateToken(token);
	};

	let auth = async () => {
		try {
			let deviceCodeAuth = await startDeviceCodeAuth();
			this.authState = {
				type: "authing",
				url: deviceCodeAuth.link_url,
				qr: deviceCodeAuth.qr_svg_uri,
			};

			let token = await deviceCodeAuth.token;
			authStore.token = token;
			await populateToken(token);
		} catch (err) {
			this.authState = { type: "error", error: err };
		}
	};
	let deauth = async () => {
		authStore.token = undefined;
		this.authState = { type: "ready" };
	};

	let authInfo = (): MinecraftAuthInfo => {
		if (this.authState.type === "done") {
			return {
				playerName: this.authState.name,
				uuid: this.authState.uuid,
				accessToken: this.authState.token,
				userType: "msa",
			};
		}
		if (import.meta.env.DEV) {
			return {
				playerName: "ikvmcraft",
				uuid: "",
				accessToken: "0",
				userType: "legacy",
			};
		}
		throw new Error("no auth");
	};

	return (
		<div>
			<Logo />
			<p>
				This is a port of modern Minecraft, including drop-in mods, to the
				browser using a custom threaded .NET 10 build for WebAssembly with a
				just-in-time compiler and a custom fork of IKVM, a JVM and
				Bytecode-to-IL translator for .NET. It may not run well on low-end
				devices due to Minecraft being translated to WASM fully at runtime,
				allowing for full mods support but resulting in some performance loss.
			</p>
			<p>
				You will need to own Minecraft on a Microsoft account and sign in to use
				this port. It will download Minecraft automatically into the browser
				filesystem before launching. The port supports all versions of Minecraft
				from 1.13 to 26.2, with more coming soon.
			</p>
			<p>
				The source is available on{" "}
				<a href="https://github.com/r58playz/ikvmcraft" target="_blank">
					GitHub
				</a>
				. NOT AN OFFICIAL MINECRAFT PRODUCT. NOT APPROVED BY OR ASSOCIATED WITH
				MOJANG OR MICROSOFT.
			</p>
			<h2>Authentication</h2>
			<div class="auth">
				<Button type="normal" icon="left" on:click={deauth}>
					<Icon icon={iconLogout} />
					Log Out
				</Button>
				<div>
					{use(this.authState).map((x) => {
						if (x.type === "loading") return <span>Loading auth state...</span>;
						else if (x.type === "ready")
							return (
								<Button type="primary" icon="left" on:click={auth}>
									<Icon icon={iconLogin} />
									Log In
								</Button>
							);
						else if (x.type === "done")
							return <span>Logged in as {x.name}</span>;
						else if (x.type === "authing")
							return (
								<div>
									<div>
										Log in at{" "}
										<a href={x.url} target="_blank">
											{x.url}
										</a>
									</div>
									<img class="qr" src={x.qr} />
								</div>
							);
						else if (x.type === "error") return <span>Error: {x.error}</span>;
					})}
				</div>
			</div>
			<h2>Play</h2>
			<div class="settings">
				<div>
					<span>Minecraft version: </span>
					<TextField value={use(this.version)} placeholder={ver} />
				</div>
				<div>
					<span>Fabric Loader version: </span>
					<TextField value={use(this.fabric)} placeholder={fab} />
				</div>
				<div class="options">
					<Button
						type="primary"
						icon="left"
						class="play"
						disabled={readyToPlay.not()}
						on:click={() => this.play(authInfo())}
					>
						<Icon icon={iconPlayArrow} />
						Play
					</Button>
					<Button
						type="normal"
						icon="left"
						on:click={() => (this.fsOpen = !this.fsOpen)}
					>
						<Icon icon={iconFolderOpen} />
						Files
					</Button>
				</div>
			</div>
			<h2>License</h2>
			<div>
				This project is licensed under the <a href="https://www.gnu.org/licenses/agpl-3.0.en.html">GNU AGPL v3</a>.
			</div>
			<pre>{`r58Playz/ikvmcraft
Copyright (C) 2026 Toshit Chawda

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.`}
			</pre>
			<OpfsExplorer open={use(this.fsOpen)} />
		</div>
	);
}
Launcher.style = css`
	:scope {
		margin: auto;

		max-width: 50rem;
		padding: 1rem;
	}

	.settings {
		display: flex;
		flex-direction: column;
		gap: 0.5rem;
	}

	.options {
		display: flex;
		flex-direction: row;
		gap: 0.5rem;
	}
	.options :global(.play) {
		flex: 1;
	}

	.auth {
		display: flex;
		flex-direction: column;
		gap: 1rem;
		align-items: start;
	}

	.auth .qr {
		width: 16rem;
		height: 16rem;
		padding: 1rem;
		background: white;
	}

	pre { white-space: pre-wrap; }
`;

function App(
	this: FC<
		{},
		{
			version: string;
			fabric: string;
			runner: ComponentInstance<typeof Runner>;
		}
	>
) {
	this.version = "26.2";
	this.fabric = "0.19.5";

	let play = async (info: MinecraftAuthInfo) => {
		this.runner = (
			<Runner version={this.version} fabric={this.fabric} auth={info} />
		) as ComponentInstance<typeof Runner>;
	};

	return (
		<div>
			<div class="launcher" class:hidden={use(this.runner).map((x) => !!x)}>
				<Launcher
					version={use(this.version)}
					fabric={use(this.fabric)}
					play={play}
				/>
			</div>
			<div>{use(this.runner)}</div>
		</div>
	);
}
App.style = css`
	:scope {
		position: relative;
		height: 100%;
		width: 100%;
		background: var(--bg);
	}

	.launcher {
		z-index: 1;
	}

	:scope > * {
		position: absolute;
		inset: 0;
		overflow-y: scroll;
	}

	.hidden {
		display: none;
	}
`;

document.querySelector("#app")!.replaceWith(<App />);
