function toHex(bytes: Uint8Array): string {
	return Array.from(bytes, (byte) => byte.toString(16).padStart(2, "0")).join("");
}

self.addEventListener("message", async (event: MessageEvent<{ id: number; data: ArrayBuffer }>) => {
	const { id, data } = event.data;
	try {
		const digest = await crypto.subtle.digest("SHA-1", data);
		const hash = toHex(new Uint8Array(digest));
		self.postMessage({ id, hash });
	} catch (error) {
		const message = error instanceof Error ? error.message : String(error);
		self.postMessage({ id, error: message });
	}
});
