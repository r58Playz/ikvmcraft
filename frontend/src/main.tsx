import { css, FC } from "dreamland/core";
import "./style.css";
import { dotnetState, initDotnet, play } from "./dotnet";
import { downloadMinecraftVersionToOpfs, isMinecraftVersionDownloaded } from "./minecraft";

function App(this: FC<{}, { canvas: HTMLCanvasElement }>) {
	this.cx.mount = async () => {
		await initDotnet(this.canvas);

		if (!(await isMinecraftVersionDownloaded("1.16.1", { verifyHashes: true })))
			await downloadMinecraftVersionToOpfs("1.16.1");
		else
			console.debug("downlaodead 1.16.1 already");

		await play();
	};

	return (
		<div>
			<canvas id="canvas" class="canvas" this={use(this.canvas)} />
			{use(dotnetState.logs).mapEach(x => <div>{x}</div>)}
		</div>
	)
}
App.style = css`
	:scope {
		overflow: scroll;
		height: 100%;
	}
`;

document.querySelector("#app")!.replaceWith(<App />);
