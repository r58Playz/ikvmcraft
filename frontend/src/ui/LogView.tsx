import { css, type FC } from "dreamland/core";
import { loglisteners, type Log } from "../dotnet/log";

const BLOCK_LINES = 200;

export function LogView(this: FC<{ scrolling: boolean }>) {
	this.cx.mount = () => {
		const content = this.root.firstElementChild!;
		const pending: Log[] = [];
		let frame = 0;
		let block: HTMLDivElement | undefined;
		let blockLines = 0;

		const flush = () => {
			frame = 0;
			const fragment = document.createDocumentFragment();
			for (const { color, log } of pending) {
				for (const text of log.split(/\r\n?|\n/)) {
					if (!block) {
						block = document.createElement("div");
						block.className = "log-block";
						fragment.append(block);
					}
					const line = document.createElement("div");
					line.className = `log log-${color}`;
					line.textContent = text;
					block.append(line);
					if (++blockLines === BLOCK_LINES) {
						// Only immutable blocks may cache their off-screen height.
						block.classList.add("complete");
						block = undefined;
						blockLines = 0;
					}
				}
			}
			content.append(fragment);
			pending.length = 0;
		};

		loglisteners.push((log) => {
			pending.push(log);
			if (!frame) frame = requestAnimationFrame(flush);
		});
	};

	return (
		<div
			class="component-log"
			style={this.scrolling ? "overflow: auto" : "overflow: hidden"}
		>
			<div class="log-content" />
		</div>
	);
}
LogView.style = css`
	:scope {
		min-height: 0;
		flex: 1;
		contain: strict;
		font-family: var(--font-mono);
		line-height: 1.25;

		/* Keep the scroll origin at the bottom. Native scroll anchoring holds
		   the reader's position when they scroll back through older output. */
		display: flex;
		flex-direction: column-reverse;

		padding: 0.5rem;
		background: var(--bg-sub);
		border-top: 0.1rem solid var(--surface4);
	}

	.log-content {
		/* One normally ordered child preserves search and selection order.
		   The auto margin keeps short logs at the top of the view. */
		flex: none;
		margin-block-end: auto;
	}
	.log-content :global(.log-block.complete) {
		content-visibility: auto;
		contain-intrinsic-block-size: auto calc(${String(BLOCK_LINES)} * 1lh);
	}
	.log-content :global(.log) {
		white-space: pre-wrap;
		overflow-wrap: anywhere;
		min-block-size: 1lh;
	}
	.log-content :global(.log-error) { color: var(--error); }
	.log-content :global(.log-warning) { color: var(--warning); }
	.log-content :global(.log-default) { color: var(--fg); }
	.log-content :global(.log-info) { color: var(--info); }
	.log-content :global(.log-debug) { color: var(--fg4); }

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
