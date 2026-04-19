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
		proxy: {
			"/proxy/meta": {
				target: "https://meta.prismlauncher.org",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/meta/, ""),
			},
			"/proxy/piston-meta": {
				target: "https://piston-meta.mojang.com",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/piston-meta/, ""),
			},
			"/proxy/piston-data": {
				target: "https://piston-data.mojang.com",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/piston-data/, ""),
			},
			"/proxy/launcher": {
				target: "https://launcher.mojang.com",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/launcher/, ""),
			},
			"/proxy/resources": {
				target: "https://resources.download.minecraft.net",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/resources/, ""),
			},
			"/proxy/libraries": {
				target: "https://libraries.minecraft.net",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/libraries/, ""),
			},
			"/proxy/maven-central": {
				target: "https://repo1.maven.org",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/maven-central/, ""),
			},
			"/proxy/maven-central-alt": {
				target: "https://repo.maven.apache.org",
				changeOrigin: true,
				rewrite: (path) => path.replace(/^\/proxy\/maven-central-alt/, ""),
			},
		},
		port: 5021,
		strictPort: true,
		host: true,
		allowedHosts: ["nyatop.internal.hgci.org", "100.64.0.10"]
	},
});
