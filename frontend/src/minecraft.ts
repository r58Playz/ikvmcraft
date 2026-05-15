const DEFAULT_PRISM_META_BASE_URL = "https://meta.prismlauncher.org/v1/";
const DEFAULT_RESOURCE_BASE_URL = "https://resources.download.minecraft.net/";
const DEFAULT_LIBRARY_BASE_URL = "https://libraries.minecraft.net/";
const DEFAULT_OPFS_ROOT_DIRECTORY = "ikvmcraft";
const DEFAULT_MAX_CONCURRENT_ASSET_DOWNLOADS = 8;
const DEFAULT_MAX_CONCURRENT_LIBRARY_DOWNLOADS = 8;
const DEFAULT_MAX_CONCURRENT_ASSET_CHECKS = 16;
const DEFAULT_SHA1_WORKER_COUNT = 4;
const DEFAULT_MINECRAFT_OS_NAME = "linux";

const DEV_PROXY_PREFIX_BY_HOST: Record<string, string> = {
	"meta.prismlauncher.org": "/proxy/meta",
	"piston-meta.mojang.com": "/proxy/piston-meta",
	"piston-data.mojang.com": "/proxy/piston-data",
	"launcher.mojang.com": "/proxy/launcher",
	"resources.download.minecraft.net": "/proxy/resources",
	"libraries.minecraft.net": "/proxy/libraries",
	"repo1.maven.org": "/proxy/maven-central",
	"repo.maven.apache.org": "/proxy/maven-central-alt",
};

interface PrismVersionList {
	versions: PrismVersionEntry[];
}

interface PrismVersionEntry {
	version: string;
}

interface DownloadInfo {
	url: string;
	sha1?: string;
	size?: number;
	path?: string;
}

interface MinecraftRule {
	action?: string;
	os?: {
		name?: string;
	};
}

interface PrismMinecraftVersion {
	id?: string;
	version?: string;
	assets?: string;
	downloads?: {
		client?: DownloadInfo;
	};
	mainJar?: {
		downloads?: {
			artifact?: DownloadInfo;
		};
	};
	assetIndex?: DownloadInfo & {
		id?: string;
	};
	libraries?: PrismMinecraftLibrary[];
}

interface PrismMinecraftLibrary {
	name?: string;
	downloads?: {
		artifact?: DownloadInfo;
	};
	rules?: MinecraftRule[];
}

interface AssetIndexObject {
	hash: string;
	size?: number;
}

interface AssetIndex {
	objects: Record<string, AssetIndexObject>;
}

interface AssetObjectDownload {
	hash: string;
	size?: number;
}

interface LibraryDownload {
	name: string;
	relativePath: string;
	url: string;
	sha1?: string;
	size?: number;
}

interface IntegrityExpectation {
	sha1?: string;
	size?: number;
}

export interface DownloadMinecraftVersionToOpfsOptions {
	prismMetaBaseUrl?: string;
	resourceBaseUrl?: string;
	libraryBaseUrl?: string;
	opfsRootDirectory?: string;
	maxConcurrentAssetDownloads?: number;
	maxConcurrentLibraryDownloads?: number;
	minecraftOsName?: string;
}

export interface DownloadMinecraftVersionToOpfsResult {
	version: string;
	versionJsonPath: string;
	clientJarPath: string;
	libraryCount: number;
	librariesDirectoryPath: string;
	assetIndexId: string;
	assetIndexPath: string;
	assetObjectCount: number;
	assetObjectsDirectoryPath: string;
}

export interface IsMinecraftVersionDownloadedOptions {
	opfsRootDirectory?: string;
	verifyAssetObjects?: boolean;
	verifyHashes?: boolean;
	minecraftOsName?: string;
	maxConcurrentAssetChecks?: number;
}

function ensureTrailingSlash(url: string): string {
	return url.endsWith("/") ? url : `${url}/`;
}

function ensurePositiveInteger(value: number, fallback: number): number {
	if (!Number.isFinite(value)) {
		return fallback;
	}

	const normalized = Math.floor(value);
	return normalized > 0 ? normalized : fallback;
}

function ensureSafePathSegment(segment: string, label: string): string {
	const trimmed = segment.trim();
	if (trimmed.length === 0) {
		throw new Error(`Invalid ${label}: segment is empty.`);
	}

	if (trimmed === "." || trimmed === ".." || trimmed.includes("/") || trimmed.includes("\\")) {
		throw new Error(`Invalid ${label}: '${segment}'.`);
	}

	return trimmed;
}

function normalizeMinecraftOsName(osName: string | undefined): string {
	if (osName === undefined) {
		return DEFAULT_MINECRAFT_OS_NAME;
	}

	const normalized = osName.trim().toLowerCase();
	if (normalized === "emscripten") {
		return "linux";
	}

	return normalized.length === 0 ? DEFAULT_MINECRAFT_OS_NAME : normalized;
}

function splitPath(path: string): string[] {
	const parts = path.split("/").filter((part) => part.length > 0);
	if (parts.length === 0) {
		throw new Error("OPFS path must not be empty.");
	}

	for (const part of parts) {
		ensureSafePathSegment(part, "path segment");
	}

	return parts;
}

function ensureResponseOk(response: Response, url: string): void {
	if (!response.ok) {
		throw new Error(`Request failed for '${url}': HTTP ${response.status} ${response.statusText}`);
	}
}

function toProxyPath(url: URL): string {
	if (url.origin === location.origin) {
		return `${url.pathname}${url.search}${url.hash}`;
	}

	const prefix = DEV_PROXY_PREFIX_BY_HOST[url.hostname];
	if (prefix === undefined) {
		throw new Error(`No Vite dev proxy route configured for host '${url.hostname}'.`);
	}

	return `${prefix}${url.pathname}${url.search}${url.hash}`;
}

async function proxyFetch(input: RequestInfo | URL, init?: RequestInit): Promise<Response> {
	const baseRequest = input instanceof Request ? input : new Request(input, init);
	const request = init === undefined ? baseRequest : new Request(baseRequest, init);
	const requestUrl = new URL(request.url, location.href);
	const proxiedRequest = new Request(toProxyPath(requestUrl), request);
	return fetch(proxiedRequest);
}

async function fetchJson<T>(url: string): Promise<T> {
	const response = await proxyFetch(url);
	ensureResponseOk(response, url);
	return await response.json() as T;
}

async function fetchText(url: string): Promise<string> {
	const response = await proxyFetch(url);
	ensureResponseOk(response, url);
	return await response.text();
}

async function fetchArrayBuffer(url: string): Promise<ArrayBuffer> {
	const response = await proxyFetch(url);
	ensureResponseOk(response, url);
	return await response.arrayBuffer();
}

type Sha1WorkerRequest = {
	id: number;
	data: ArrayBuffer;
};

type Sha1WorkerResponse = {
	id: number;
	hash?: string;
	error?: string;
};

type Sha1WorkerTask = {
	id: number;
	data: ArrayBuffer;
	resolve: (hash: string) => void;
	reject: (error: Error) => void;
};

class Sha1WorkerPool {
	private readonly workers: Worker[];
	private readonly idleWorkers: Worker[];
	private readonly inflight = new Map<number, { resolve: (hash: string) => void; reject: (error: Error) => void }>();
	private readonly workerToTaskId = new Map<Worker, number>();
	private readonly queue: Sha1WorkerTask[] = [];
	private nextId = 1;

	constructor(workerCount: number) {
		this.workers = Array.from({ length: workerCount }, () => {
			return new Worker(new URL("./sha1.worker.ts", import.meta.url), { type: "module" });
		});
		this.idleWorkers = [...this.workers];
		for (const worker of this.workers) {
			worker.addEventListener("message", (event) => {
				this.handleMessage(worker, event as MessageEvent<Sha1WorkerResponse>);
			});
			worker.addEventListener("error", (event) => {
				this.handleError(worker, event as ErrorEvent);
			});
		}
	}

	hash(data: ArrayBuffer): Promise<string> {
		return new Promise((resolve, reject) => {
			const task: Sha1WorkerTask = {
				id: this.nextId,
				data,
				resolve,
				reject,
			};
			this.nextId += 1;
			this.queue.push(task);
			this.pump();
		});
	}

	private pump(): void {
		while (this.idleWorkers.length > 0 && this.queue.length > 0) {
			const worker = this.idleWorkers.pop();
			const task = this.queue.shift();
			if (worker === undefined || task === undefined) {
				return;
			}
			this.inflight.set(task.id, { resolve: task.resolve, reject: task.reject });
			this.workerToTaskId.set(worker, task.id);
			worker.postMessage({ id: task.id, data: task.data } satisfies Sha1WorkerRequest);
		}
	}

	private handleMessage(worker: Worker, event: MessageEvent<Sha1WorkerResponse>): void {
		const { id, hash, error } = event.data;
		const callbacks = this.inflight.get(id);
		if (callbacks !== undefined) {
			this.inflight.delete(id);
			if (error !== undefined) {
				callbacks.reject(new Error(error));
			} else if (hash !== undefined) {
				callbacks.resolve(hash);
			} else {
				callbacks.reject(new Error("SHA-1 worker returned no result."));
			}
		}
		this.workerToTaskId.delete(worker);
		this.idleWorkers.push(worker);
		this.pump();
	}

	private handleError(worker: Worker, event: ErrorEvent): void {
		const taskId = this.workerToTaskId.get(worker);
		if (taskId !== undefined) {
			const callbacks = this.inflight.get(taskId);
			this.inflight.delete(taskId);
			this.workerToTaskId.delete(worker);
			this.idleWorkers.push(worker);
			this.pump();
			callbacks?.reject(event.error instanceof Error ? event.error : new Error(event.message));
			return;
		}
		console.warn("[minecraft] SHA-1 worker error.", event);
	}
}

function resolveSha1WorkerCount(): number {
	if (typeof navigator === "undefined") {
		return DEFAULT_SHA1_WORKER_COUNT;
	}
	const hardwareCount = navigator.hardwareConcurrency ?? DEFAULT_SHA1_WORKER_COUNT;
	return Math.max(1, Math.min(DEFAULT_SHA1_WORKER_COUNT, hardwareCount));
}

function createSha1WorkerPool(): Sha1WorkerPool | null {
	if (typeof Worker === "undefined") {
		return null;
	}
	try {
		return new Sha1WorkerPool(resolveSha1WorkerCount());
	} catch (error) {
		console.warn("[minecraft] Failed to initialize SHA-1 worker pool.", error);
		return null;
	}
}

const sha1WorkerPool = createSha1WorkerPool();

async function getSha1HexInline(data: ArrayBuffer): Promise<string> {
	const digest = await crypto.subtle.digest("SHA-1", data);
	const bytes = new Uint8Array(digest);
	return Array.from(bytes, (byte) => byte.toString(16).padStart(2, "0")).join("");
}

async function getSha1Hex(data: ArrayBuffer): Promise<string> {
	if (sha1WorkerPool === null) {
		return await getSha1HexInline(data);
	}
	try {
		return await sha1WorkerPool.hash(data);
	} catch (error) {
		console.warn("[minecraft] SHA-1 worker failed, falling back to main thread.", error);
		return await getSha1HexInline(data);
	}
}

async function assertIntegrity(
	data: ArrayBuffer,
	expectation: IntegrityExpectation,
	context: string,
): Promise<void> {
	const expectedSize = expectation.size;
	if (expectedSize !== undefined && data.byteLength !== expectedSize) {
		throw new Error(`Size mismatch for ${context}: expected ${expectedSize}, got ${data.byteLength}.`);
	}

	const expectedSha1 = expectation.sha1;
	if (expectedSha1 !== undefined) {
		const actualSha1 = await getSha1Hex(data);
		if (actualSha1 !== expectedSha1.toLowerCase()) {
			throw new Error(`SHA-1 mismatch for ${context}: expected ${expectedSha1}, got ${actualSha1}.`);
		}
	}
}

async function downloadBinary(url: string, expectation: IntegrityExpectation, context: string): Promise<ArrayBuffer> {
	const data = await fetchArrayBuffer(url);
	await assertIntegrity(data, expectation, context);
	return data;
}

async function getOrCreateDirectory(
	root: FileSystemDirectoryHandle,
	pathSegments: string[],
): Promise<FileSystemDirectoryHandle> {
	let current = root;
	for (const segment of pathSegments) {
		current = await current.getDirectoryHandle(segment, { create: true });
	}
	return current;
}

function isNotFoundError(error: unknown): boolean {
	return error instanceof DOMException && error.name === "NotFoundError";
}

async function getDirectoryIfExists(
	root: FileSystemDirectoryHandle,
	pathSegments: string[],
): Promise<FileSystemDirectoryHandle | null> {
	let current = root;
	for (const segment of pathSegments) {
		try {
			current = await current.getDirectoryHandle(segment);
		} catch (error) {
			if (isNotFoundError(error)) {
				return null;
			}

			throw error;
		}
	}

	return current;
}

async function getFileHandleIfExists(
	root: FileSystemDirectoryHandle,
	path: string,
): Promise<FileSystemFileHandle | null> {
	const segments = splitPath(path);
	const fileName = segments[segments.length - 1];
	if (fileName === undefined) {
		throw new Error(`Cannot resolve file name for path '${path}'.`);
	}

	const parentDirectory = await getDirectoryIfExists(root, segments.slice(0, -1));
	if (parentDirectory === null) {
		return null;
	}

	try {
		return await parentDirectory.getFileHandle(fileName);
	} catch (error) {
		if (isNotFoundError(error)) {
			return null;
		}

		throw error;
	}
}

async function fileExists(root: FileSystemDirectoryHandle, path: string): Promise<boolean> {
	return (await getFileHandleIfExists(root, path)) !== null;
}

async function readFileText(root: FileSystemDirectoryHandle, path: string): Promise<string> {
	const fileHandle = await getFileHandleIfExists(root, path);
	if (fileHandle === null) {
		throw new Error(`Missing file '${path}'.`);
	}

	const file = await fileHandle.getFile();
	return await file.text();
}

async function readFileArrayBuffer(root: FileSystemDirectoryHandle, path: string): Promise<ArrayBuffer> {
	const fileHandle = await getFileHandleIfExists(root, path);
	if (fileHandle === null) {
		throw new Error(`Missing file '${path}'.`);
	}

	const file = await fileHandle.getFile();
	return await file.arrayBuffer();
}

async function writeFileToOpfs(
	root: FileSystemDirectoryHandle,
	path: string,
	content: FileSystemWriteChunkType,
): Promise<void> {
	const segments = splitPath(path);
	const fileName = segments[segments.length - 1];
	if (fileName === undefined) {
		throw new Error(`Cannot resolve file name for path '${path}'.`);
	}

	const directory = await getOrCreateDirectory(root, segments.slice(0, -1));
	const fileHandle = await directory.getFileHandle(fileName, { create: true });
	const writable = await fileHandle.createWritable();
	try {
		await writable.write(content);
	} finally {
	await writable.close();
}

function warnDownloadCheckFailure(context: string, error?: unknown): void {
	if (error === undefined) {
		console.warn(`[minecraft] ${context}`);
		return;
	}
	console.warn(`[minecraft] ${context}`, error);
}
}

function collectAssetObjects(assetIndex: AssetIndex): AssetObjectDownload[] {
	if (assetIndex.objects === undefined) {
		throw new Error("Asset index is missing 'objects'.");
	}

	const byHash = new Map<string, number | undefined>();
	for (const object of Object.values(assetIndex.objects)) {
		const hash = object.hash.toLowerCase();
		if (!/^[0-9a-f]{40}$/.test(hash)) {
			throw new Error(`Invalid asset hash '${object.hash}'.`);
		}

		if (!byHash.has(hash)) {
			byHash.set(hash, object.size);
		}
	}

	return Array.from(byHash, ([hash, size]) => ({ hash, size }));
}

function resolveClientDownload(versionManifest: PrismMinecraftVersion): DownloadInfo | undefined {
	return versionManifest.downloads?.client ?? versionManifest.mainJar?.downloads?.artifact;
}

function shouldIncludeLibrary(rules: MinecraftRule[] | undefined, minecraftOsName: string): boolean {
	if (rules === undefined || rules.length === 0) {
		return true;
	}

	let allowed = false;
	for (const rule of rules) {
		let matchesOs = true;
		if (rule.os?.name !== undefined) {
			matchesOs = normalizeMinecraftOsName(rule.os.name) === minecraftOsName;
		}

		if (!matchesOs) {
			continue;
		}

		allowed = (rule.action ?? "allow").toLowerCase() === "allow";
	}

	return allowed;
}

function parseMavenLibraryPath(libraryName: string): string {
	const segments = libraryName.split(":");
	if (segments.length < 3) {
		throw new Error(`Cannot parse Maven coordinates '${libraryName}'.`);
	}

	const group = segments[0];
	const artifact = segments[1];
	const version = segments[2];
	const classifier = segments.length > 3 && segments[3].trim().length > 0 ? `-${segments[3].trim()}` : "";

	const groupPath = group.split(".").join("/");
	return `${groupPath}/${artifact}/${version}/${artifact}-${version}${classifier}.jar`;
}

function resolveLibraryRelativePath(library: PrismMinecraftLibrary): string {
	const downloadPath = library.downloads?.artifact?.path;
	if (downloadPath !== undefined) {
		return downloadPath;
	}

	if (library.name === undefined) {
		throw new Error("Library entry is missing both 'downloads.artifact.path' and 'name'.");
	}

	return parseMavenLibraryPath(library.name);
}

function resolveLibraryUrl(library: PrismMinecraftLibrary, relativePath: string, libraryBaseUrl: string): string {
	const artifactUrl = library.downloads?.artifact?.url;
	if (artifactUrl !== undefined) {
		return artifactUrl;
	}

	return new URL(relativePath, libraryBaseUrl).toString();
}

function collectLibraryDownloads(
	libraries: PrismMinecraftLibrary[] | undefined,
	libraryBaseUrl: string,
	minecraftOsName: string,
): LibraryDownload[] {
	if (libraries === undefined) {
		return [];
	}

	const byRelativePath = new Map<string, LibraryDownload>();
	for (const library of libraries) {
		if (!shouldIncludeLibrary(library.rules, minecraftOsName)) {
			continue;
		}

		const relativePath = resolveLibraryRelativePath(library);
		const url = resolveLibraryUrl(library, relativePath, libraryBaseUrl);
		const resolved = {
			name: library.name ?? relativePath,
			relativePath,
			url,
			sha1: library.downloads?.artifact?.sha1,
			size: library.downloads?.artifact?.size,
		} satisfies LibraryDownload;

		if (!byRelativePath.has(relativePath)) {
			byRelativePath.set(relativePath, resolved);
		}
	}

	return Array.from(byRelativePath.values());
}

async function runConcurrently<T>(
	items: readonly T[],
	concurrency: number,
	worker: (item: T, index: number) => Promise<void>,
): Promise<void> {
	if (items.length === 0) {
		return;
	}

	let cursor = 0;
	const workerCount = Math.min(concurrency, items.length);
	const runners = Array.from({ length: workerCount }, async () => {
		while (true) {
			const index = cursor;
			cursor += 1;
			if (index >= items.length) {
				return;
			}

			await worker(items[index], index);
		}
	});

	await Promise.all(runners);
}

export async function downloadMinecraftVersionToOpfs(
	version: string,
	options: DownloadMinecraftVersionToOpfsOptions = {},
): Promise<DownloadMinecraftVersionToOpfsResult> {
	const prismMetaBaseUrl = ensureTrailingSlash(options.prismMetaBaseUrl ?? DEFAULT_PRISM_META_BASE_URL);
	const resourceBaseUrl = ensureTrailingSlash(options.resourceBaseUrl ?? DEFAULT_RESOURCE_BASE_URL);
	const libraryBaseUrl = ensureTrailingSlash(options.libraryBaseUrl ?? DEFAULT_LIBRARY_BASE_URL);
	const opfsRootDirectory = ensureSafePathSegment(
		options.opfsRootDirectory ?? DEFAULT_OPFS_ROOT_DIRECTORY,
		"OPFS root directory",
	);
	const minecraftOsName = normalizeMinecraftOsName(options.minecraftOsName);
	const maxConcurrentAssetDownloads = ensurePositiveInteger(
		options.maxConcurrentAssetDownloads ?? DEFAULT_MAX_CONCURRENT_ASSET_DOWNLOADS,
		DEFAULT_MAX_CONCURRENT_ASSET_DOWNLOADS,
	);
	const maxConcurrentLibraryDownloads = ensurePositiveInteger(
		options.maxConcurrentLibraryDownloads ?? DEFAULT_MAX_CONCURRENT_LIBRARY_DOWNLOADS,
		DEFAULT_MAX_CONCURRENT_LIBRARY_DOWNLOADS,
	);

	const requestedVersion = ensureSafePathSegment(version, "version");
	const versionListUrl = new URL("net.minecraft/index.json", prismMetaBaseUrl).toString();
	const versionList = await fetchJson<PrismVersionList>(versionListUrl);
	const selectedVersion = versionList.versions.find((entry) => entry.version === requestedVersion);
	if (selectedVersion === undefined) {
		throw new Error(`Minecraft version '${requestedVersion}' was not found in Prism metadata.`);
	}

	const versionManifestUrl = new URL(
		`net.minecraft/${encodeURIComponent(selectedVersion.version)}.json`,
		prismMetaBaseUrl,
	).toString();
	const versionManifestText = await fetchText(versionManifestUrl);
	const versionManifest = JSON.parse(versionManifestText) as PrismMinecraftVersion;

	const clientDownload = resolveClientDownload(versionManifest);
	if (clientDownload === undefined) {
		throw new Error(`Version '${selectedVersion.version}' does not define a client download (checked 'downloads.client' and 'mainJar.downloads.artifact').`);
	}

	const assetIndexDownload = versionManifest.assetIndex;
	if (assetIndexDownload === undefined) {
		throw new Error(
			`Version '${selectedVersion.version}' does not define 'assetIndex' (legacy assets pipeline is not supported).`,
		);
	}

	const resolvedVersion = ensureSafePathSegment(
		versionManifest.id ?? versionManifest.version ?? selectedVersion.version,
		"resolved version",
	);
	const assetIndexId = ensureSafePathSegment(
		assetIndexDownload.id ?? versionManifest.assets ?? resolvedVersion,
		"asset index id",
	);

	const root = await navigator.storage.getDirectory();
	const versionJsonPath = `${opfsRootDirectory}/versions/${resolvedVersion}/${resolvedVersion}.json`;
	await writeFileToOpfs(root, versionJsonPath, versionManifestText);

	const libraries = collectLibraryDownloads(versionManifest.libraries, libraryBaseUrl, minecraftOsName);
	await runConcurrently(libraries, maxConcurrentLibraryDownloads, async (library) => {
		const libraryData = await downloadBinary(
			library.url,
			{ sha1: library.sha1, size: library.size },
			`library '${library.name}'`,
		);
		const libraryPath = `${opfsRootDirectory}/libraries/${library.relativePath}`;
		await writeFileToOpfs(root, libraryPath, libraryData);
	});

	const clientJarData = await downloadBinary(clientDownload.url, clientDownload, `client jar '${resolvedVersion}'`);
	const clientJarPath = `${opfsRootDirectory}/versions/${resolvedVersion}/${resolvedVersion}.jar`;
	await writeFileToOpfs(root, clientJarPath, clientJarData);

	const assetIndexData = await downloadBinary(
		assetIndexDownload.url,
		assetIndexDownload,
		`asset index '${assetIndexId}'`,
	);
	const assetIndexText = new TextDecoder().decode(assetIndexData);
	const parsedAssetIndex = JSON.parse(assetIndexText) as AssetIndex;
	const assetIndexPath = `${opfsRootDirectory}/assets/indexes/${assetIndexId}.json`;
	await writeFileToOpfs(root, assetIndexPath, assetIndexText);

	const assetObjects = collectAssetObjects(parsedAssetIndex);
	await runConcurrently(assetObjects, maxConcurrentAssetDownloads, async (assetObject) => {
		const prefix = assetObject.hash.slice(0, 2);
		const objectUrl = new URL(`${prefix}/${assetObject.hash}`, resourceBaseUrl).toString();
		const objectPath = `${opfsRootDirectory}/assets/objects/${prefix}/${assetObject.hash}`;
		const objectData = await downloadBinary(
			objectUrl,
			{ sha1: assetObject.hash, size: assetObject.size },
			`asset object '${assetObject.hash}'`,
		);
		await writeFileToOpfs(root, objectPath, objectData);
	});

	return {
		version: resolvedVersion,
		versionJsonPath,
		clientJarPath,
		libraryCount: libraries.length,
		librariesDirectoryPath: `${opfsRootDirectory}/libraries`,
		assetIndexId,
		assetIndexPath,
		assetObjectCount: assetObjects.length,
		assetObjectsDirectoryPath: `${opfsRootDirectory}/assets/objects`,
	};
}

export async function isMinecraftVersionDownloaded(
	version: string,
	options: IsMinecraftVersionDownloadedOptions = {},
): Promise<boolean> {
	const resolvedVersion = ensureSafePathSegment(version, "version");
	const opfsRootDirectory = ensureSafePathSegment(
		options.opfsRootDirectory ?? DEFAULT_OPFS_ROOT_DIRECTORY,
		"OPFS root directory",
	);
	const minecraftOsName = normalizeMinecraftOsName(options.minecraftOsName);
	const verifyAssetObjects = options.verifyAssetObjects ?? true;
	const verifyHashes = options.verifyHashes ?? false;
	const maxConcurrentAssetChecks = ensurePositiveInteger(
		options.maxConcurrentAssetChecks ?? DEFAULT_MAX_CONCURRENT_ASSET_CHECKS,
		DEFAULT_MAX_CONCURRENT_ASSET_CHECKS,
	);

	const root = await navigator.storage.getDirectory();
	const versionJsonPath = `${opfsRootDirectory}/versions/${resolvedVersion}/${resolvedVersion}.json`;
	const clientJarPath = `${opfsRootDirectory}/versions/${resolvedVersion}/${resolvedVersion}.jar`;
	const versionJsonExists = await fileExists(root, versionJsonPath);
	const clientJarExists = await fileExists(root, clientJarPath);
	if (!versionJsonExists) {
		warnDownloadCheckFailure(`Missing version manifest at '${versionJsonPath}'.`);
	}
	if (!clientJarExists) {
		warnDownloadCheckFailure(`Missing client jar at '${clientJarPath}'.`);
	}
	if (!versionJsonExists || !clientJarExists) {
		return false;
	}

	let versionManifest: PrismMinecraftVersion;
	try {
		const manifestText = await readFileText(root, versionJsonPath);
		versionManifest = JSON.parse(manifestText) as PrismMinecraftVersion;
	} catch (error) {
		warnDownloadCheckFailure(`Failed to read version manifest at '${versionJsonPath}'.`, error);
		return false;
	}

	const assetIndexDownload = versionManifest.assetIndex;
	if (assetIndexDownload === undefined) {
		warnDownloadCheckFailure(`Version manifest for '${resolvedVersion}' is missing asset index metadata.`);
		return false;
	}

	const clientDownload = resolveClientDownload(versionManifest);
	if (clientDownload === undefined) {
		warnDownloadCheckFailure(`Version manifest for '${resolvedVersion}' is missing client download metadata.`);
		return false;
	}

	const libraries = collectLibraryDownloads(versionManifest.libraries, DEFAULT_LIBRARY_BASE_URL, minecraftOsName);
	for (const library of libraries) {
		const libraryPath = `${opfsRootDirectory}/libraries/${library.relativePath}`;
		try {
			if (verifyHashes) {
				const libraryData = await readFileArrayBuffer(root, libraryPath);
				await assertIntegrity(libraryData, { sha1: library.sha1, size: library.size }, `library '${library.name}'`);
			} else if (!(await fileExists(root, libraryPath))) {
				warnDownloadCheckFailure(`Missing library '${library.name}' at '${libraryPath}'.`);
				return false;
			}
		} catch (error) {
			warnDownloadCheckFailure(`Library check failed for '${library.name}'.`, error);
			return false;
		}
	}

	try {
		if (verifyHashes) {
			const clientJarData = await readFileArrayBuffer(root, clientJarPath);
			await assertIntegrity(
				clientJarData,
				{ sha1: clientDownload.sha1, size: clientDownload.size },
				`client jar '${resolvedVersion}'`,
			);
		}
	} catch (error) {
		warnDownloadCheckFailure(`Client jar check failed for '${resolvedVersion}'.`, error);
		return false;
	}

	let assetIndexId: string;
	try {
		assetIndexId = ensureSafePathSegment(
			assetIndexDownload.id ?? versionManifest.assets ?? resolvedVersion,
			"asset index id",
		);
	} catch (error) {
		warnDownloadCheckFailure(`Invalid asset index id for '${resolvedVersion}'.`, error);
		return false;
	}

	const assetIndexPath = `${opfsRootDirectory}/assets/indexes/${assetIndexId}.json`;
	try {
		if (verifyHashes) {
			const assetIndexData = await readFileArrayBuffer(root, assetIndexPath);
			await assertIntegrity(
				assetIndexData,
				{ sha1: assetIndexDownload.sha1, size: assetIndexDownload.size },
				`asset index '${assetIndexId}'`,
			);
		} else if (!(await fileExists(root, assetIndexPath))) {
			warnDownloadCheckFailure(`Missing asset index at '${assetIndexPath}'.`);
			return false;
		}
	} catch (error) {
		warnDownloadCheckFailure(`Asset index check failed for '${assetIndexId}'.`, error);
		return false;
	}

	if (!verifyAssetObjects) {
		return true;
	}

	let assetObjects: AssetObjectDownload[];
	try {
		const assetIndexText = await readFileText(root, assetIndexPath);
		const assetIndex = JSON.parse(assetIndexText) as AssetIndex;
		assetObjects = collectAssetObjects(assetIndex);
	} catch (error) {
		warnDownloadCheckFailure(`Failed to read asset index at '${assetIndexPath}'.`, error);
		return false;
	}

	let assetCheckError: Error | null = null;
	await runConcurrently(assetObjects, maxConcurrentAssetChecks, async (assetObject) => {
		if (assetCheckError !== null) {
			return;
		}

		const hashPrefix = assetObject.hash.slice(0, 2);
		const objectPath = `${opfsRootDirectory}/assets/objects/${hashPrefix}/${assetObject.hash}`;
		console.debug("[verify] path", objectPath);
		try {
			if (verifyHashes) {
				const objectData = await readFileArrayBuffer(root, objectPath);
				await assertIntegrity(
					objectData,
					{ sha1: assetObject.hash, size: assetObject.size },
					`asset object '${assetObject.hash}'`,
				);
			} else if (!(await fileExists(root, objectPath))) {
				assetCheckError = new Error(`Missing asset object '${assetObject.hash}'.`);
			}
		} catch (error) {
			assetCheckError = error instanceof Error ? error : new Error(String(error));
		}
	});

	if (assetCheckError !== null) {
		warnDownloadCheckFailure("Asset object check failed.", assetCheckError);
		return false;
	}

	return true;
}
