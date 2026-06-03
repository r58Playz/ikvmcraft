let EitherSocketProvider$1 = class EitherSocketProvider {};
let JsProvider$1 = class JsProvider {};
class WispProvider {}
class WasmProvider {}
class WasmWispProvider {}
class ProtocolExtensionBuilders {}
let UdpProtocolExtensionBuilder$1 = class UdpProtocolExtensionBuilder {};
let UdpProtocolExtensionBuilderRef$1 = class UdpProtocolExtensionBuilderRef {};
let UdpProtocolExtension$1 = class UdpProtocolExtension {};
let MotdProtocolExtensionBuilder$1 = class MotdProtocolExtensionBuilder {};
let MotdProtocolExtensionBuilderRef$1 = class MotdProtocolExtensionBuilderRef {};
let MotdProtocolExtension$1 = class MotdProtocolExtension {};
let PasswordProtocolExtensionBuilder$1 = class PasswordProtocolExtensionBuilder {};
let PasswordProtocolExtensionBuilderRef$1 = class PasswordProtocolExtensionBuilderRef {};
let PasswordProtocolExtension$1 = class PasswordProtocolExtension {};
let CertAuthProtocolExtensionBuilder$1 = class CertAuthProtocolExtensionBuilder {};
let CertAuthProtocolExtensionBuilderRef$1 = class CertAuthProtocolExtensionBuilderRef {};
let CertAuthProtocolExtension$1 = class CertAuthProtocolExtension {};
class WsReadEvent {}
class WsWriteEvent {}

type Role = "client" | "server";
declare class TransportRead {
}
declare class TransportWrite {
}
declare abstract class ProtocolExtension {
    abstract readonly id: number;
}
declare abstract class ProtocolExtensionBuilder {
    abstract readonly id: number;
}
declare abstract class JsProtocolExtension extends ProtocolExtension {
    readonly id: number;
    constructor(id: number, packets: number[], congestionStreams: number[]);
    abstract encode(): Uint8Array;
    handleHandshake(read: TransportRead, write: TransportWrite): Promise<void> | void;
    handlePacket(type: number, packet: Uint8Array, read: TransportRead, write: TransportWrite): Promise<void> | void;
}
declare abstract class JsProtocolExtensionBuilder extends ProtocolExtensionBuilder {
    readonly id: number;
    constructor(id: number);
    abstract buildFromBytes(bytes: Uint8Array, role: Role): JsProtocolExtension;
    abstract buildToExtension(role: Role): JsProtocolExtension;
    appendTo(builders: ProtocolExtensionBuilders): void;
}

declare class UdpProtocolExtensionBuilder extends ProtocolExtensionBuilder {
    inner: UdpProtocolExtensionBuilder$1;
    readonly id = 1;
    private constructor();
    static client(): UdpProtocolExtensionBuilder;
    static server(): UdpProtocolExtensionBuilder;
    appendTo(builders: ProtocolExtensionBuilders): void;
}
declare class UdpProtocolExtensionBuilderRef extends ProtocolExtensionBuilder {
    inner: UdpProtocolExtensionBuilderRef$1;
    readonly id = 1;
    constructor(inner: UdpProtocolExtensionBuilderRef$1);
    appendTo(_builders: ProtocolExtensionBuilders): void;
}
declare class UdpProtocolExtension extends ProtocolExtension {
    inner: UdpProtocolExtension$1;
    readonly id = 1;
    constructor(inner: UdpProtocolExtension$1);
}
declare class MotdProtocolExtensionBuilder extends ProtocolExtensionBuilder {
    inner: MotdProtocolExtensionBuilder$1;
    readonly id = 4;
    private constructor();
    static client(): MotdProtocolExtensionBuilder;
    static server(motd: string): MotdProtocolExtensionBuilder;
    appendTo(builders: ProtocolExtensionBuilders): void;
}
declare class MotdProtocolExtensionBuilderRef extends ProtocolExtensionBuilder {
    inner: MotdProtocolExtensionBuilderRef$1;
    readonly id = 4;
    constructor(inner: MotdProtocolExtensionBuilderRef$1);
    appendTo(_builders: ProtocolExtensionBuilders): void;
    get motd(): string | undefined;
    get isClient(): boolean;
}
declare class MotdProtocolExtension extends ProtocolExtension {
    inner: MotdProtocolExtension$1;
    readonly id = 4;
    constructor(inner: MotdProtocolExtension$1);
    get motd(): string;
}
declare class PasswordProtocolExtensionBuilder extends ProtocolExtensionBuilder {
    inner: PasswordProtocolExtensionBuilder$1;
    readonly id = 2;
    private constructor();
    static client(user?: string, password?: string): PasswordProtocolExtensionBuilder;
    static server(required?: boolean): PasswordProtocolExtensionBuilder;
    appendTo(builders: ProtocolExtensionBuilders): void;
}
declare class PasswordProtocolExtensionBuilderRef extends ProtocolExtensionBuilder {
    inner: PasswordProtocolExtensionBuilderRef$1;
    readonly id = 2;
    constructor(inner: PasswordProtocolExtensionBuilderRef$1);
    appendTo(_builders: ProtocolExtensionBuilders): void;
    get required(): boolean | undefined;
}
declare class PasswordProtocolExtension extends ProtocolExtension {
    inner: PasswordProtocolExtension$1;
    readonly id = 2;
    constructor(inner: PasswordProtocolExtension$1);
    get required(): boolean | undefined;
    get user(): string | undefined;
    get chosenUser(): string | undefined;
}
declare class CertAuthProtocolExtensionBuilder extends ProtocolExtensionBuilder {
    inner: CertAuthProtocolExtensionBuilder$1;
    readonly id = 3;
    private constructor();
    static client(): CertAuthProtocolExtensionBuilder;
    static server(required?: boolean): CertAuthProtocolExtensionBuilder;
    appendTo(builders: ProtocolExtensionBuilders): void;
}
declare class CertAuthProtocolExtensionBuilderRef extends ProtocolExtensionBuilder {
    inner: CertAuthProtocolExtensionBuilderRef$1;
    readonly id = 3;
    constructor(inner: CertAuthProtocolExtensionBuilderRef$1);
    appendTo(_builders: ProtocolExtensionBuilders): void;
    get required(): boolean | undefined;
}
declare class CertAuthProtocolExtension extends ProtocolExtension {
    inner: CertAuthProtocolExtension$1;
    readonly id = 3;
    constructor(inner: CertAuthProtocolExtension$1);
    get required(): boolean | undefined;
}

declare class WispExtensions {
    get(idx: number): ProtocolExtension | undefined;
    get length(): number;
    drop(): void;
}

type ProviderResult = [
    readable: ReadableStream<Uint8Array<ArrayBuffer>>,
    writable: WritableStream<Uint8Array<ArrayBuffer>>
];
declare abstract class Provider<T, P> {
    constructor(provider: T, map: (provider: T) => P);
}
declare class JsProvider extends Provider<JsProvider$1, WasmWispProvider> {
    constructor(func: (host: string) => Promise<ProviderResult> | ProviderResult);
}
declare class WebSocketJsProvider extends JsProvider {
    constructor();
}
declare class JsSocketProvider extends Provider<JsProvider$1, WasmProvider> {
    constructor(func: (host: string, port: number) => Promise<ProviderResult> | ProviderResult);
}
declare class WsProxyJsSocketProvider extends JsSocketProvider {
    constructor(wsproxy: string);
}
interface WispV2Handshake {
    builders: ProtocolExtensionBuilder[];
}
declare class WispSocketProvider extends Provider<WispProvider, WasmProvider> {
    clone(): WispProvider;
    constructor(provider: JsProvider, server: string, connectionPrefs?: () => [
        v2: WispV2Handshake | undefined,
        requiredExts: number[]
    ]);
    replaceMux(): Promise<void>;
    getExtensions(): Promise<WispExtensions | undefined>;
}
declare class EitherSocketProvider extends Provider<EitherSocketProvider$1, WasmProvider> {
    constructor(selector: (host: string, port: number) => "left" | "right", left: SocketProvider, right: SocketProvider);
}
type SocketProvider = JsSocketProvider | WispSocketProvider | EitherSocketProvider;

type EpoxyRawHeaders = Record<string, string[]>;
type EpoxyResponse = Response & {
    rawHeaders: EpoxyRawHeaders;
};

type EpoxyWSChunk = string | Uint8Array<ArrayBufferLike>;
interface EpoxyWSCloseInfo {
    closeCode?: number;
    reason?: string;
}
interface EpoxyWebSocketOptions {
    protocols?: string | string[];
    headers?: HeadersInit;
}
declare class EpoxyWS {
    #private;
    readonly readable: ReadableStream<EpoxyWSChunk>;
    readonly writable: WritableStream<EpoxyWSChunk>;
    readonly protocol: string;
    readonly headers: Headers;
    readonly rawHeaders: EpoxyRawHeaders;
    readonly closed: Promise<EpoxyWSCloseInfo>;
    constructor(readable: ReadableStream<WsReadEvent>, writable: WritableStream<WsWriteEvent>, protocol: string, headers: Headers, rawHeaders: EpoxyRawHeaders);
    close(closeInfo?: EpoxyWSCloseInfo): void;
}

declare class EpoxyClient {
    constructor(provider: SocketProvider, redirectLimit?: number);
    get userAgent(): string;
    set userAgent(val: string);
    fetch(resource: Request | URL | string, options?: RequestInit): Promise<EpoxyResponse>;
    websocket(resource: string | URL, options?: EpoxyWebSocketOptions): Promise<EpoxyWS>;
    connect(host: string, port: number, bufferSize?: number): Promise<TcpStream>;
    connectTls(host: string, port: number, bufferSize?: number): Promise<TlsStream>;
}
declare class TcpStream {
    read: ReadableStream<Uint8Array>;
    write: WritableStream<Uint8Array>;
}
declare class TlsStream {
    read: ReadableStream<Uint8Array>;
    write: WritableStream<Uint8Array>;
}

declare let version: {
    package: string;
    git: string;
};

type EpoxyInitInput = RequestInfo | URL | Response | BufferSource | WebAssembly.Module | Promise<RequestInfo | URL | Response | BufferSource | WebAssembly.Module> | {
    module_or_path: RequestInfo | URL | Response | BufferSource | WebAssembly.Module | Promise<RequestInfo | URL | Response | BufferSource | WebAssembly.Module>;
};
declare function init(input: EpoxyInitInput): Promise<void>;

export { CertAuthProtocolExtension, CertAuthProtocolExtensionBuilder, CertAuthProtocolExtensionBuilderRef, EitherSocketProvider, EpoxyClient, EpoxyWS, JsProtocolExtension, JsProtocolExtensionBuilder, JsProvider, JsSocketProvider, MotdProtocolExtension, MotdProtocolExtensionBuilder, MotdProtocolExtensionBuilderRef, PasswordProtocolExtension, PasswordProtocolExtensionBuilder, PasswordProtocolExtensionBuilderRef, ProtocolExtension, ProtocolExtensionBuilder, TransportRead, TransportWrite, UdpProtocolExtension, UdpProtocolExtensionBuilder, UdpProtocolExtensionBuilderRef, WebSocketJsProvider, WispExtensions, WispSocketProvider, WsProxyJsSocketProvider, init, version };
export type { EpoxyInitInput, EpoxyRawHeaders, EpoxyResponse, EpoxyWSChunk, EpoxyWSCloseInfo, EpoxyWebSocketOptions, ProviderResult, Role };
