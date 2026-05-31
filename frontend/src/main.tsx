import { css, FC } from "dreamland/core";
import "./style.css";
import { loglisteners, initDotnet, play } from "./dotnet";
import { downloadFabricMinecraftVersionToOpfs, downloadMinecraftVersionToOpfs, isMinecraftVersionDownloaded } from "./minecraft";

function LogView(this: FC<{ scrolling: boolean }>) {
	const create = (color: string, log: string) => {
		const el = document.createElement("div");
		el.classList.add("log");
		el.innerText = log;
		el.style.color = color;
		return el;
	};

	this.cx.mount = () => {
		const logroot = this.root as HTMLElement;
		const frag = document.createDocumentFragment();

		loglisteners.push((x) => frag.append(create(x.color, x.log)));
		setInterval(() => {
			if (frag.children.length > 0) {
				logroot.appendChild(frag);
				logroot.scrollTop = logroot.scrollHeight;
			}
		}, 250);
	};

	return (
		<div
			class="component-log"
			style={this.scrolling ? "overflow: auto" : "overflow: hidden"}
		/>
	);
};
LogView.css = `
	min-height: 0;
	flex: 1;
	font-family: var(--font-mono);

	::-webkit-scrollbar {
		width: 10px;
	}
	::-webkit-scrollbar-track {
		background: var(--surface3);
	}
	::-webkit-scrollbar-thumb {
		background: var(--surface6);
	}
`;

function App(this: FC<{}, { canvas: HTMLCanvasElement }>) {
	this.cx.mount = async () => {
		if (!(await isMinecraftVersionDownloaded("1.16.1-fabric-0.19.2", { verifyHashes: true })))
			await downloadFabricMinecraftVersionToOpfs("1.16.1", { loaderVersion: "0.19.2" });
		else
			console.debug("downlaodead 1.16.1 fabric 0.19.2 already");

		await initDotnet(this.canvas);

		await play("1.16.1-fabric-0.19.2");
	};

	return (
		<div>
			<canvas id="canvas" class="canvas" this={use(this.canvas)} />
			<LogView scrolling={true} />
		</div>
	)
}
App.style = css`
	:scope {
		height: 100%;
		display: flex;
		flex-direction: column;
		align-items: center;
	}

	:global(.component-log) {
		font-family: monospace;
		flex: 1;
		align-self: stretch;
		background: #eee;
	}
`;

document.querySelector("#app")!.replaceWith(<App />);
