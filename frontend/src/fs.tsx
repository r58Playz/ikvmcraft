import { css, FC } from "dreamland/core";

// The File System Access API pickers are non-standard and missing from lib.dom.
declare global {
	function showOpenFilePicker(options?: { multiple?: boolean }): Promise<FileSystemFileHandle[]>;
	function showDirectoryPicker(): Promise<FileSystemDirectoryHandle>;
	interface Window {
		showOpenFilePicker?: unknown;
		showDirectoryPicker?: unknown;
	}
}

export const PICKERS_UNAVAILABLE =
	!window.showDirectoryPicker || !window.showOpenFilePicker;

const rootFolder = await navigator.storage.getDirectory();

async function recursiveGetDirectory(
	dir: FileSystemDirectoryHandle,
	path: string[]
): Promise<FileSystemDirectoryHandle> {
	if (path.length === 0) return dir;
	return recursiveGetDirectory(await dir.getDirectoryHandle(path[0]), path.slice(1));
}

async function copyFile(file: FileSystemFileHandle, to: FileSystemDirectoryHandle) {
	const data = await file.getFile().then((r) => r.stream());
	const handle = await to.getFileHandle(file.name, { create: true });
	const writable = await handle.createWritable();
	await data.pipeTo(writable);
}

async function copyFolder(folder: FileSystemDirectoryHandle, to: FileSystemDirectoryHandle) {
	async function upload(from: FileSystemDirectoryHandle, into: FileSystemDirectoryHandle) {
		// FileSystemDirectoryHandle async iteration lives in lib.dom.asynciterable, not enabled here.
		for await (const [name, entry] of from as any) {
			if (entry.kind === "file") {
				await copyFile(entry as FileSystemFileHandle, into);
			} else {
				const newTo = await into.getDirectoryHandle(name, { create: true });
				await upload(entry as FileSystemDirectoryHandle, newTo);
			}
		}
	}
	const newFolder = await to.getDirectoryHandle(folder.name, { create: true });
	await upload(folder, newFolder);
}

type Entry = { name: string; entry: FileSystemHandle };

export function OpfsExplorer(
	this: FC<
		{ open: boolean },
		{
			path: FileSystemDirectoryHandle;
			components: string[];
			entries: Entry[];
			editing: FileSystemFileHandle | null;
			uploading: boolean;
		}
	>
) {
	this.path = rootFolder;
	this.components = [];
	this.entries = [];
	this.editing = null as FileSystemFileHandle | null;
	this.uploading = false;

	const refresh = async () => {
		this.components = (await rootFolder.resolve(this.path)) || [];

		const entries: Entry[] = [];
		for await (const [name, entry] of this.path as any) {
			entries.push({ name, entry: entry as FileSystemHandle });
		}
		entries.sort((a, b) => {
			const kind = a.entry.kind.localeCompare(b.entry.kind);
			return kind === 0 ? a.name.localeCompare(b.name) : kind;
		});
		this.entries = entries;
	};

	this.cx.mount = () => {
		// reassigning this.path re-runs refresh, so navigation/upload just set this.path = this.path
		use(this.path).listen(refresh);
		use(this.open).listen((open) => {
			if (open) refresh();
		});
		refresh();
	};

	const goUp = async () => {
		this.editing = null;
		this.path = await recursiveGetDirectory(rootFolder, this.components.slice(0, -1));
	};

	const uploadFile = async () => {
		const files = await showOpenFilePicker({ multiple: true });
		this.uploading = true;
		for (const file of files) await copyFile(file, this.path);
		this.path = this.path;
		this.uploading = false;
	};

	const uploadFolder = async () => {
		const folder = await showDirectoryPicker();
		this.uploading = true;
		await copyFolder(folder, this.path);
		this.path = this.path;
		this.uploading = false;
	};

	const uploadDisabled = use(this.uploading).map((x) => x || PICKERS_UNAVAILABLE);

	return (
		<div class="fs-explorer" class:open={use(this.open)}>
			<div class="bar">
				{use(this.components).map((c) =>
					c.length > 0 ? (
						<button on:click={goUp} title="Up a level">
							..
						</button>
					) : null
				)}
				<span class="cwd">
					{use(this.components).map((c) => (c.length === 0 ? "/" : "/" + c.join("/")))}
				</span>
				<span class="spacer" />
				<button disabled={uploadDisabled} on:click={uploadFile}>
					Upload File
				</button>
				<button disabled={uploadDisabled} on:click={uploadFolder}>
					Upload Folder
				</button>
				<button on:click={() => (this.open = false)}>Close</button>
			</div>

			{use(this.uploading).map((u) => (u ? <div class="status">Uploading files...</div> : null))}

			<div class="entries">
				{use(this.entries).map((entries) =>
					entries.map((x) => {
						const isDir = x.entry.kind === "directory";

						const open = () => {
							if (isDir) {
								this.editing = null;
								this.path = x.entry as FileSystemDirectoryHandle;
							} else {
								this.editing = x.entry as FileSystemFileHandle;
							}
						};
						const download = async () => {
							const blob = await (x.entry as FileSystemFileHandle).getFile();
							const url = URL.createObjectURL(blob);
							const a = document.createElement("a");
							a.href = url;
							a.download = x.name;
							a.click();
							await new Promise((r) => setTimeout(r, 100));
							URL.revokeObjectURL(url);
						};
						const remove = async () => {
							if (this.editing?.name === x.name) this.editing = null;
							await this.path.removeEntry(x.name, { recursive: true });
							this.path = this.path;
						};

						return (
							<div class="entry">
								<button class="open" on:click={open}>
									<span class="kind">{isDir ? "dir" : "file"}</span>
									{x.name}
								</button>
								{!isDir ? (
									<button on:click={download} title="Download file">
										download
									</button>
								) : null}
								<button on:click={remove} title="Delete">
									delete
								</button>
							</div>
						);
					})
				)}
			</div>

			{use(this.editing).map((file) => {
				if (!file) return null;

				const area = (<textarea />) as HTMLTextAreaElement;
				area.value = "Loading file...";
				file
					.getFile()
					.then((r) => r.text())
					.then((r) => (area.value = r));

				const save = async () => {
					const writable = await file.createWritable();
					await writable.write(area.value);
					await writable.close();
					this.editing = null;
				};

				return (
					<div class="editor">
						<div class="controls">
							<span class="name">{file.name}</span>
							<span class="spacer" />
							<button on:click={save}>Save</button>
							<button on:click={() => (this.editing = null)}>Close</button>
						</div>
						{area}
					</div>
				);
			})}
		</div>
	);
}
OpfsExplorer.style = css`
	:scope {
		display: none;
		position: fixed;
		top: 0;
		right: 0;
		bottom: 0;
		width: min(32rem, 100%);
		flex-direction: column;
		gap: 0.5rem;
		padding: 0.75rem;
		background: #fff;
		border-left: 1px solid #000;
		font-family: monospace;
		overflow: auto;
		z-index: 10;
	}
	:scope.open {
		display: flex;
	}

	.bar {
		display: flex;
		align-items: center;
		gap: 0.5rem;
		flex-wrap: wrap;
	}
	.bar .cwd {
		font-weight: bold;
		word-break: break-all;
	}
	.spacer {
		flex: 1;
	}

	.status {
		font-style: italic;
	}

	.entries {
		display: flex;
		flex-direction: column;
		gap: 0.25rem;
		min-height: 0;
	}
	.entry {
		display: flex;
		align-items: center;
		gap: 0.5rem;
	}
	.entry .open {
		flex: 1;
		text-align: left;
		font-family: monospace;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
	}
	.entry .kind {
		color: #888;
		margin-right: 0.5rem;
	}

	.editor {
		display: flex;
		flex-direction: column;
		gap: 0.5rem;
	}
	.editor .controls {
		display: flex;
		align-items: center;
		gap: 0.5rem;
	}
	.editor .name {
		font-weight: bold;
		word-break: break-all;
	}
	.editor textarea {
		min-height: 16rem;
		font-family: monospace;
	}
`;
