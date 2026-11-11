import { init, EpoxyClient, WebSocketJsProvider, WispSocketProvider } from "@mercuryworkshop/epoxy-tls/full";
import wasm from "@mercuryworkshop/epoxy-tls/full/wasm?url";

export let client: EpoxyClient;

await init({ module_or_path: wasm });

// The Wisp server: epoxy tunnels the frontend's own HTTPS through it (auth, downloads), and Java's sockets reach it
// through the runtime's transport (loader/wasmfs-wisp-transport.js, IKVM_WISP_URL). Under the vite dev server that is
// the server itself (the `local-wisp` plugin in vite.config.ts); a static production build has no such endpoint and
// keeps the public one. `?wisp=<url>` overrides either, e.g. `?wisp=wss://anura.pro/` to compare against the external
// proxy.
export function wispUrl(): string {
	const override = new URLSearchParams(location.search).get("wisp");
	if (override) return override;
	if (import.meta.env.DEV) return `${location.protocol === "https:" ? "wss:" : "ws:"}//${location.host}/wisp/`;
	return "wss://anura.pro/";
}

export function createClient() {
	const url = wispUrl();
	console.log("wisp server", url);
	let wisp = new WispSocketProvider(new WebSocketJsProvider(), url);
	client = new EpoxyClient(wisp);
	(globalThis as any).client = client;
}

createClient();
(globalThis as any).createEpoxyClient = createClient;
