import { css, FC } from "dreamland/core";
import "./style.css";
import { dotnetState, initDotnet, play } from "./dotnet";

function App(this: FC<{}, { canvas: HTMLCanvasElement }>) {
	this.cx.mount = async () => {
		await initDotnet(this.canvas);
		await play();
	};

	return (
		<div>
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
