using System.Runtime.InteropServices;

class icalls {
	[DllImport("icalls")]
    static internal extern long wasm_icall_lii(int a, int b);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liii(int a, int b, int c);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliii(int a, int b, long c, int d, int e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viil(int a, int b, long c);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viili(int a, int b, long c, int d);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilii(int a, int b, long c, int d, int e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiili(int a, int b, int c, long d, int e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilli(int a, int b, int c, int d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liil(int a, int b, long c);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiill(int a, int b, int c, long d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiill(int a, int b, long c, long d);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liili(int a, int b, long c, int d);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liill(int a, int b, long c, long d);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiillll(int a, int b, int c, int d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliil(int a, int b, long c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliill(int a, int b, long c, int d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllll(int a, int b, long c, long d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilll(int a, int b, long c, long d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillll(int a, int b, long c, long d, long e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viill(int a, int b, long c, long d);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiil(int a, int b, int c, int d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiil(int a, int b, int c, long d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiffff(int a, int b, float c, float d, float e, float f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllll(int a, int b, long c, long d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillli(int a, int b, long c, int d, long e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiill(int a, int b, int c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilli(int a, int b, int c, long d, long e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiil(int a, int b, int c, int d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilll(int a, int b, int c, long d, long e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiill(int a, int b, int c, int d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiililil(int a, int b, int c, long d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilll(int a, int b, long c, long d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiili(int a, int b, int c, int d, int e, long f, int g);
}
