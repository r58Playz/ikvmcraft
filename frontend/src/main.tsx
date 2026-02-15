import { FC } from "dreamland/core";
import "./style.css";
import { initDotnet, play } from "./dotnet";

function App(this: FC<{}, { canvas: HTMLCanvasElement }>) {
	this.cx.mount = async () => {
		await initDotnet(this.canvas);
		await play();
	};

	return (
		<div>
			<canvas id="canvas" class="canvas" this={use(this.canvas)} />
		</div>
	)
}

document.querySelector("#app")!.replaceWith(<App />);
