import { defineConfig } from "vite";
import basicSsl from "@vitejs/plugin-basic-ssl";

export default defineConfig({
	plugins: [basicSsl()],
	build: {
		target: "es2022",
	},
	server: {
		headers: {
			"Cross-Origin-Opener-Policy": "same-origin",
			"Cross-Origin-Embedder-Policy": "require-corp",
		},
		port: 5021,
		strictPort: true,
		host: true,
		allowedHosts: ["nyatop.internal.hgci.org", "100.64.0.10"]
	},
});
