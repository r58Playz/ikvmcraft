// The socket transport for WasmFS's AF_INET/AF_INET6 sockets (FNA-WASM-Build emsdk.12, src/lib/libwasmfs_sockets.js
// has the contract): TCP streams over one Wisp connection per worker, with wisp-js's client.
//
// Linked with --pre-js, so it is defined in every worker; emsdk.12 calls it only on its network pthread, and every
// event below is delivered there (the Wisp WebSocket belongs to that worker). Two environment variables, which the
// frontend sets, configure it: IKVM_WISP_URL (the Wisp server) and IKVM_WISP_CLIENT_URL (wisp-js's self-contained
// dist/wisp-client.mjs, served next to the app).
//
// What Wisp v1 cannot express, and so this does not offer: a connect acknowledgement (open is reported once the Wisp
// connection is up, and a refused or unreachable host arrives as a close with an errno), half-close (shutdown), and
// receive-side flow control (pause/resume: the server does not wait for the client).
Module['wasmfsSocketTransport'] = (() => {
  // Wisp CLOSE reasons -> the names emsdk.12 maps to errno. 0x02 (voluntary) is an orderly close.
  const closeErrno = (reason) => {
    switch (reason) {
      case 0x01: case 0x02: return 0;
      case 0x42: return 'EHOSTUNREACH';
      case 0x43: case 0x47: return 'ETIMEDOUT';
      case 0x44: case 0x48: case 0x49: return 'ECONNREFUSED';
      default: return 'ECONNRESET';
    }
  };
  // The C environment: the runtime setenv()s the config's environmentVariables at startup, into the one environ every
  // pthread shares. (Module.config, where main keeps them, read back nothing on the network pthread's worker.)
  const env = (name) => {
    const n = lengthBytesUTF8(name) + 1, p = _malloc(n);
    try {
      stringToUTF8(name, p, n);
      const v = _mono_wasm_getenv(p);
      if (!v) return undefined;
      try { return UTF8ToString(v); } finally { _free(v); }
    } finally {
      _free(p);
    }
  };
  // err() is emscripten's printErr, which a pthread forwards to the page's console
  let announced = false;

  let wisp = null;          // the wisp-js client module, once imported
  let conn = null;          // the ClientConnection, while it is up or connecting
  let connReady = null;     // resolves once conn has its first CONTINUE (Wisp's "ready")
  const streams = new Map(); // socket id -> { stream, events }

  async function connection () {
    if (conn && conn.connected) return conn;
    if (connReady) return connReady;
    connReady = (async () => {
      const url = env('IKVM_WISP_URL'), client = env('IKVM_WISP_CLIENT_URL');
      if (!url || !client) throw new Error('IKVM_WISP_URL / IKVM_WISP_CLIENT_URL are not set');
      wisp ??= (await import(/* webpackIgnore: true */ client)).client;
      const c = new wisp.ClientConnection(url);
      await new Promise((resolve, reject) => {
        c.onopen = resolve;
        c.onerror = () => reject(new Error(`wisp: cannot connect to ${url}`));
      });
      // the WebSocket went away: every stream on it is gone (cleanup() closes them with reason 0x03)
      c.onclose = () => { if (conn === c) { conn = null; connReady = null; } };
      conn = c;
      return c;
    })();
    try {
      return await connReady;
    } catch (e) {
      connReady = null;
      throw e;
    }
  }

  return {
    'connect' (id, host, port, family, events) {
      if (!announced) {
        announced = true;
        err(`[wisp transport] first connect ${host}:${port}; wisp ${env('IKVM_WISP_URL')}, client ${env('IKVM_WISP_CLIENT_URL')}`);
      }
      connection().then((c) => {
        const stream = c.create_stream(host, port, 0x01);
        streams.set(id, { stream, events });
        stream.onmessage = (bytes) => events['data'](id, bytes);
        stream.onclose = (reason) => {
          if (!streams.delete(id)) return;
          const err = closeErrno(reason);
          if (!err) events['end'](id);
          events['close'](id, err);
        };
        // DATA that waited for CONTINUE credit leaves wisp-js's send_buffer here; report it drained then
        const cont = stream.continue_received.bind(stream);
        stream.continue_received = (n) => {
          const before = stream.send_buffer.reduce((a, b) => a + b.byteLength, 0);
          cont(n);
          const after = stream.send_buffer.reduce((a, b) => a + b.byteLength, 0);
          if (before > after) events['drained'](id, before - after);
        };
        events['open'](id);
      }, (e) => {
        err(`[wisp transport] ${host}:${port}: ${e && e.stack || e}`);
        events['close'](id, 'ENETUNREACH');
      });
    },
    'send' (id, bytes) {
      const s = streams.get(id);
      if (!s) return;
      const queued = s.stream.buffer_size <= 0;
      s.stream.send(bytes);
      // sent now: it is the WebSocket's; queued: drained when CONTINUE releases it (above)
      if (!queued) s.events['drained'](id, bytes.byteLength);
    },
    'close' (id, abort) {
      const s = streams.get(id);
      if (!s) return;
      streams.delete(id);
      s.stream.close(abort ? 0x03 : 0x02);
    },
  };
})();
