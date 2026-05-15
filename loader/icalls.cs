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
    static internal extern void wasm_icall_viiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, long k);
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
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiiil(int a, int b, long c, long d, int e, int f, int g, int h, int i, long j);
	[DllImport("icalls")]
    static internal extern double wasm_icall_diil(int a, int b, long c);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillil(int a, int b, long c, long d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilill(int a, int b, long c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllli(int a, int b, long c, long d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiif(int a, int b, int c, float d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiif(int a, int b, int c, int d, float e);
    [DllImport("icalls")]
    static internal extern double wasm_icall_dii(int a, int b);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diid(int a, int b, double c);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diidd(int a, int b, double c, double d);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diii(int a, int b, int c);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diiil(int a, int b, int c, long d);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diili(int a, int b, long c, int d);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diiliil(int a, int b, long c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diilil(int a, int b, long c, int d, long e);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diill(int a, int b, long c, long d);
    [DllImport("icalls")]
    static internal extern double wasm_icall_diilll(int a, int b, long c, long d, long e);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiffffffi(int a, int b, float c, float d, float e, float f, float g, float h, int i);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiifffffi(int a, int b, float c, float d, float e, float f, float g, int h);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiifffiii(int a, int b, float c, float d, float e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiifffiiii(int a, int b, float c, float d, float e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiffll(int a, int b, float c, float d, long e, long f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiifll(int a, int b, float c, long d, long e);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiii(int a, int b, int c);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiiiil(int a, int b, int c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiiill(int a, int b, int c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiil(int a, int b, int c, long d);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiilfl(int a, int b, int c, long d, float e, long f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiilli(int a, int b, int c, long d, long e, int f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiil(int a, int b, long c);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiilf(int a, int b, long c, float d);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiilffl(int a, int b, long c, float d, float e, long f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiilfl(int a, int b, long c, float d, long e);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiilflil(int a, int b, long c, float d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiili(int a, int b, long c, int d);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiiliil(int a, int b, long c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiilil(int a, int b, long c, int d, long e);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiilill(int a, int b, long c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiill(int a, int b, long c, long d);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiilll(int a, int b, long c, long d, long e);
    [DllImport("icalls")]
    static internal extern float wasm_icall_fiillll(int a, int b, long c, long d, long e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiid(int a, int b, double c);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiidi(int a, int b, double c, int d);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiif(int a, int b, float c);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiffl(int a, int b, float c, float d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiffllli(int a, int b, float c, float d, long e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiifiiifiiii(int a, int b, float c, int d, int e, int f, float g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiifiiifiiiiiiiiil(int a, int b, float c, int d, int e, int f, float g, int h, int i, int j, int k, int l, int m, int n, int o, int p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiifiiiiiii(int a, int b, float c, int d, int e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiifil(int a, int b, float c, int d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiff(int a, int b, int c, float d, float e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiffl(int a, int b, int c, float d, float e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiffll(int a, int b, int c, float d, float e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiifil(int a, int b, int c, float d, int e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiifl(int a, int b, int c, float d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiidi(int a, int b, int c, int d, double e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiff(int a, int b, int c, int d, float e, float f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiffl(int a, int b, int c, int d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiifffffl(int a, int b, int c, int d, int e, float f, float g, float h, float i, float j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiffll(int a, int b, int c, int d, int e, float f, float g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiifill(int a, int b, int c, int d, int e, float f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiifllll(int a, int b, int c, int d, int e, float f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiflllll(int a, int b, int c, int d, int e, float f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiifl(int a, int b, int c, int d, int e, int f, float g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiidiiddidddddddiiii(int a, int b, int c, int d, int e, int f, int g, int h, double i, int j, int k, double l, double m, int n, double o, double p, double q, double r, double s, double t, double u, int v, int w, int x, int y);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiil(int a, int b, int c, int d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiiliiiilil(int a, int b, int c, int d, int e, int f, int g, long h, int i, int j, int k, int l, long m, int n, long o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiill(int a, int b, int c, int d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiilll(int a, int b, int c, int d, int e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiiillll(int a, int b, int c, int d, int e, int f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiil(int a, int b, int c, int d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiili(int a, int b, int c, int d, int e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiill(int a, int b, int c, int d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiilll(int a, int b, int c, int d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiillll(int a, int b, int c, int d, int e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiil(int a, int b, int c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiliiliil(int a, int b, int c, int d, int e, long f, int g, int h, long i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiiliilil(int a, int b, int c, int d, int e, long f, int g, int h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiilill(int a, int b, int c, int d, int e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiill(int a, int b, int c, int d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiilll(int a, int b, int c, int d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiilllll(int a, int b, int c, int d, int e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiil(int a, int b, int c, int d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiili(int a, int b, int c, int d, long e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilii(int a, int b, int c, int d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiliii(int a, int b, int c, int d, long e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiliiiif(int a, int b, int c, int d, long e, int f, int g, int h, int i, float j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiliiiifl(int a, int b, int c, int d, long e, int f, int g, int h, int i, float j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiiliiiiiiiiiiiiiiiiiiill(int a, int b, int c, int d, long e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t, int u, int v, int w, int x, long y, long z);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiililill(int a, int b, int c, int d, long e, int f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilill(int a, int b, int c, int d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiill(int a, int b, int c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilliiiif(int a, int b, int c, int d, long e, long f, int g, int h, int i, int j, float k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilliiiifl(int a, int b, int c, int d, long e, long f, int g, int h, int i, int j, float k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiillillll(int a, int b, int c, int d, long e, long f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilll(int a, int b, int c, int d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiillll(int a, int b, int c, int d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilllll(int a, int b, int c, int d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiillllll(int a, int b, int c, int d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiilllllll(int a, int b, int c, int d, long e, long f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiild(int a, int b, int c, long d, double e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilf(int a, int b, int c, long d, float e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilffi(int a, int b, int c, long d, float e, float f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilffl(int a, int b, int c, long d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilfl(int a, int b, int c, long d, float e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilii(int a, int b, int c, long d, int e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliifi(int a, int b, int c, long d, int e, int f, float g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliifil(int a, int b, int c, long d, int e, int f, float g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliifl(int a, int b, int c, long d, int e, int f, float g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliifll(int a, int b, int c, long d, int e, int f, float g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliii(int a, int b, int c, long d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiii(int a, int b, int c, long d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiiiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiiiiiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiiiiiiiiiifi(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, float t, int u);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiiiiiiiiiiiii(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiiil(int a, int b, int c, long d, int e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiili(int a, int b, int c, long d, int e, int f, int g, int h, long i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiiilii(int a, int b, int c, long d, int e, int f, int g, int h, long i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiil(int a, int b, int c, long d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiilii(int a, int b, int c, long d, int e, int f, int g, long h, int i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliiilll(int a, int b, int c, long d, int e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliil(int a, int b, int c, long d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliili(int a, int b, int c, long d, int e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliilill(int a, int b, int c, long d, int e, int f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliilli(int a, int b, int c, long d, int e, int f, long g, long h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliilll(int a, int b, int c, long d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiiliillll(int a, int b, int c, long d, int e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilil(int a, int b, int c, long d, int e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilili(int a, int b, int c, long d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiililiil(int a, int b, int c, long d, int e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiililil(int a, int b, int c, long d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilill(int a, int b, int c, long d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiililll(int a, int b, int c, long d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiililllill(int a, int b, int c, long d, int e, long f, long g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiililllllll(int a, int b, int c, long d, int e, long f, long g, long h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiill(int a, int b, int c, long d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilli(int a, int b, int c, long d, long e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliff(int a, int b, int c, long d, long e, int f, float g, float h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillifl(int a, int b, int c, long d, long e, int f, float g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillii(int a, int b, int c, long d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiff(int a, int b, int c, long d, long e, int f, int g, float h, float i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliifl(int a, int b, int c, long d, long e, int f, int g, float h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliii(int a, int b, int c, long d, long e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiii(int a, int b, int c, long d, long e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiiii(int a, int b, int c, long d, long e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiiiiii(int a, int b, int c, long d, long e, int f, int g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiiiiiiii(int a, int b, int c, long d, long e, int f, int g, int h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiiiiiiiiiiiiiiiiill(int a, int b, int c, long d, long e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t, int u, int v, int w, long x, long y);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiiiiiiiiiiiiiiiiilll(int a, int b, int c, long d, long e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t, int u, int v, int w, long x, long y, long z);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiilii(int a, int b, int c, long d, long e, int f, int g, int h, long i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiill(int a, int b, int c, long d, long e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliil(int a, int b, int c, long d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliili(int a, int b, int c, long d, long e, int f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiliii(int a, int b, int c, long d, long e, int f, int g, long h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiliiiiii(int a, int b, int c, long d, long e, int f, int g, long h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiliiiiiiiiiil(int a, int b, int c, long d, long e, int f, int g, long h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, long s);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliiliiiiiiiiiilfi(int a, int b, int c, long d, long e, int f, int g, long h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, long s, float t, int u);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliilil(int a, int b, int c, long d, long e, int f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliilliii(int a, int b, int c, long d, long e, int f, int g, long h, long i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillil(int a, int b, int c, long d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillilii(int a, int b, int c, long d, long e, int f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilliliii(int a, int b, int c, long d, long e, int f, long g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillilil(int a, int b, int c, long d, long e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillill(int a, int b, int c, long d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillilliii(int a, int b, int c, long d, long e, int f, long g, long h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillilll(int a, int b, int c, long d, long e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilll(int a, int b, int c, long d, long e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillli(int a, int b, int c, long d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllii(int a, int b, int c, long d, long e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillliiii(int a, int b, int c, long d, long e, long f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillliiiil(int a, int b, int c, long d, long e, long f, int g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillliiil(int a, int b, int c, long d, long e, long f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillliil(int a, int b, int c, long d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillliili(int a, int b, int c, long d, long e, long f, int g, int h, long i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllil(int a, int b, int c, long d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllili(int a, int b, int c, long d, long e, long f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillliliii(int a, int b, int c, long d, long e, long f, int g, long h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllilll(int a, int b, int c, long d, long e, long f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillll(int a, int b, int c, long d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllli(int a, int b, int c, long d, long e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllliiillilill(int a, int b, int c, long d, long e, long f, long g, int h, int i, int j, long k, long l, int m, long n, int o, long p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllliil(int a, int b, int c, long d, long e, long f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllliillilill(int a, int b, int c, long d, long e, long f, long g, int h, int i, long j, long k, int l, long m, int n, long o, long p);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllll(int a, int b, int c, long d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillllliliil(int a, int b, int c, long d, long e, long f, long g, long h, int i, long j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillllll(int a, int b, int c, long d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllllli(int a, int b, int c, long d, long e, long f, long g, long h, long i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiilllllll(int a, int b, int c, long d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiillllllll(int a, int b, int c, long d, long e, long f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiildd(int a, int b, long c, double d, double e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiildddd(int a, int b, long c, double d, double e, double f, double g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilffffffffl(int a, int b, long c, float d, float e, float f, float g, float h, float i, float j, float k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilffffl(int a, int b, long c, float d, float e, float f, float g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilffffll(int a, int b, long c, float d, float e, float f, float g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilfffl(int a, int b, long c, float d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilffiilll(int a, int b, long c, float d, float e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilffl(int a, int b, long c, float d, float e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilffll(int a, int b, long c, float d, float e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilfflll(int a, int b, long c, float d, float e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilfil(int a, int b, long c, float d, int e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilfl(int a, int b, long c, float d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilfll(int a, int b, long c, float d, long e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiili(int a, int b, long c, int d);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilididd(int a, int b, long c, int d, double e, int f, double g, double h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilidl(int a, int b, long c, int d, double e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifffffill(int a, int b, long c, int d, float e, float f, float g, float h, float i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliffffill(int a, int b, long c, int d, float e, float f, float g, float h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifffill(int a, int b, long c, int d, float e, float f, float g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifffll(int a, int b, long c, int d, float e, float f, float g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliffil(int a, int b, long c, int d, float e, float f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliffl(int a, int b, long c, int d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliffll(int a, int b, long c, int d, float e, float f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifi(int a, int b, long c, int d, float e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifiiil(int a, int b, long c, int d, float e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifl(int a, int b, long c, int d, float e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifliiiil(int a, int b, long c, int d, float e, long f, int g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliflill(int a, int b, long c, int d, float e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilifll(int a, int b, long c, int d, float e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliifffffilil(int a, int b, long c, int d, int e, float f, float g, float h, float i, float j, int k, long l, int m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiffffl(int a, int b, long c, int d, int e, float f, float g, float h, float i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiif(int a, int b, long c, int d, int e, int f, float g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiifffl(int a, int b, long c, int d, int e, int f, float g, float h, float i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiff(int a, int b, long c, int d, int e, int f, int g, float h, float i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiifl(int a, int b, long c, int d, int e, int f, int g, float h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiii(int a, int b, long c, int d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiii(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiiiiiiiiiiiiiiiiiiiii(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t, int u, int v, int w, int x, int y, int z, int p26, int p27);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiiill(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiiilll(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiiillll(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, long k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiil(int a, int b, long c, int d, int e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiiliiiiiiiiil(int a, int b, long c, int d, int e, int f, int g, int h, int i, long j, int k, int l, int m, int n, int o, int p, int q, int r, int s, long t);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiiill(int a, int b, long c, int d, int e, int f, int g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiil(int a, int b, long c, int d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiiill(int a, int b, long c, int d, int e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiil(int a, int b, long c, int d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiili(int a, int b, long c, int d, int e, int f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiilil(int a, int b, long c, int d, int e, int f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiilillll(int a, int b, long c, int d, int e, int f, int g, long h, int i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiill(int a, int b, long c, int d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiilll(int a, int b, long c, int d, int e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiil(int a, int b, long c, int d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiili(int a, int b, long c, int d, int e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiliiii(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiliiiiii(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k, int l, int m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiliiiiiii(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiliiiiiiiiiiiil(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, long t);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiliiiiiiiiiiiilffff(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, long t, float u, float v, float w, float x);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiliiiiiiiiil(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k, int l, int m, int n, int o, int p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiiliillll(int a, int b, long c, int d, int e, int f, long g, int h, int i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiill(int a, int b, long c, int d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiillil(int a, int b, long c, int d, int e, int f, long g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiilll(int a, int b, long c, int d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiillll(int a, int b, long c, int d, int e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliil(int a, int b, long c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliili(int a, int b, long c, int d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiliifiiil(int a, int b, long c, int d, int e, long f, int g, int h, float i, int j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiliiiiiiiiiii(int a, int b, long c, int d, int e, long f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliiliil(int a, int b, long c, int d, int e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliilil(int a, int b, long c, int d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliililiil(int a, int b, long c, int d, int e, long f, int g, long h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliililill(int a, int b, long c, int d, int e, long f, int g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliilill(int a, int b, long c, int d, int e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliililliil(int a, int b, long c, int d, int e, long f, int g, long h, long i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliililll(int a, int b, long c, int d, int e, long f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliilli(int a, int b, long c, int d, int e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliillii(int a, int b, long c, int d, int e, long f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliilll(int a, int b, long c, int d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliilllil(int a, int b, long c, int d, int e, long f, long g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliillll(int a, int b, long c, int d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliillllillll(int a, int b, long c, int d, int e, long f, long g, long h, long i, int j, long k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliilllll(int a, int b, long c, int d, int e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliillllll(int a, int b, long c, int d, int e, long f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliilllllll(int a, int b, long c, int d, int e, long f, long g, long h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiiliillllllll(int a, int b, long c, int d, int e, long f, long g, long h, long i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilil(int a, int b, long c, int d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilildl(int a, int b, long c, int d, long e, double f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililfl(int a, int b, long c, int d, long e, float f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilili(int a, int b, long c, int d, long e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililii(int a, int b, long c, int d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililiil(int a, int b, long c, int d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililiilil(int a, int b, long c, int d, long e, int f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililiilll(int a, int b, long c, int d, long e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililil(int a, int b, long c, int d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililili(int a, int b, long c, int d, long e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilililil(int a, int b, long c, int d, long e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililililiiil(int a, int b, long c, int d, long e, int f, long g, int h, long i, int j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililililil(int a, int b, long c, int d, long e, int f, long g, int h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililililll(int a, int b, long c, int d, long e, int f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililill(int a, int b, long c, int d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililillill(int a, int b, long c, int d, long e, int f, long g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilililll(int a, int b, long c, int d, long e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililillll(int a, int b, long c, int d, long e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilililllll(int a, int b, long c, int d, long e, int f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililillllll(int a, int b, long c, int d, long e, int f, long g, long h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililillllllll(int a, int b, long c, int d, long e, int f, long g, long h, long i, long j, long k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilill(int a, int b, long c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililli(int a, int b, long c, int d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililliiil(int a, int b, long c, int d, long e, long f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililliil(int a, int b, long c, int d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilillil(int a, int b, long c, int d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilillill(int a, int b, long c, int d, long e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilillilll(int a, int b, long c, int d, long e, long f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilillilllll(int a, int b, long c, int d, long e, long f, int g, long h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililll(int a, int b, long c, int d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililllil(int a, int b, long c, int d, long e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililllilll(int a, int b, long c, int d, long e, long f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilillll(int a, int b, long c, int d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiililllll(int a, int b, long c, int d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilillllll(int a, int b, long c, int d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillffl(int a, int b, long c, long d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillfil(int a, int b, long c, long d, float e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillfilll(int a, int b, long c, long d, float e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillfl(int a, int b, long c, long d, float e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillfllilll(int a, int b, long c, long d, float e, long f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilli(int a, int b, long c, long d, int e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliffiil(int a, int b, long c, long d, int e, float f, float g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliffil(int a, int b, long c, long d, int e, float f, float g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillifiil(int a, int b, long c, long d, int e, float f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillii(int a, int b, long c, long d, int e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliifil(int a, int b, long c, long d, int e, int f, float g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliifl(int a, int b, long c, long d, int e, int f, float g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliii(int a, int b, long c, long d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiif(int a, int b, long c, long d, int e, int f, int g, float h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiii(int a, int b, long c, long d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiiifilll(int a, int b, long c, long d, int e, int f, int g, int h, float i, int j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiiiil(int a, int b, long c, long d, int e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiiil(int a, int b, long c, long d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiiill(int a, int b, long c, long d, int e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiiilll(int a, int b, long c, long d, int e, int f, int g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiil(int a, int b, long c, long d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiili(int a, int b, long c, long d, int e, int f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiilil(int a, int b, long c, long d, int e, int f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiill(int a, int b, long c, long d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliiilll(int a, int b, long c, long d, int e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliil(int a, int b, long c, long d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliili(int a, int b, long c, long d, int e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliilil(int a, int b, long c, long d, int e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliililil(int a, int b, long c, long d, int e, int f, long g, int h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliill(int a, int b, long c, long d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliilll(int a, int b, long c, long d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliilllil(int a, int b, long c, long d, int e, int f, long g, long h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliillll(int a, int b, long c, long d, int e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillil(int a, int b, long c, long d, int e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillili(int a, int b, long c, long d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilliliil(int a, int b, long c, long d, int e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilil(int a, int b, long c, long d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillililiiil(int a, int b, long c, long d, int e, long f, int g, long h, int i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillililil(int a, int b, long c, long d, int e, long f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilililil(int a, int b, long c, long d, int e, long f, int g, long h, int i, long j, int k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilililililil(int a, int b, long c, long d, int e, long f, int g, long h, int i, long j, int k, long l, int m, long n, int o, long p);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilililill(int a, int b, long c, long d, int e, long f, int g, long h, int i, long j, int k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillililill(int a, int b, long c, long d, int e, long f, int g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilill(int a, int b, long c, long d, int e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillililll(int a, int b, long c, long d, int e, long f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillill(int a, int b, long c, long d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilldilll(int a, int b, long c, long d, int e, long f, long g, double h, int i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillfilll(int a, int b, long c, long d, int e, long f, long g, float h, int i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilliill(int a, int b, long c, long d, int e, long f, long g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilliilll(int a, int b, long c, long d, int e, long f, long g, int h, int i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillil(int a, int b, long c, long d, int e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillilll(int a, int b, long c, long d, int e, long f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilll(int a, int b, long c, long d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilllilll(int a, int b, long c, long d, int e, long f, long g, long h, int i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillll(int a, int b, long c, long d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilllldilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, double j, int k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillllfilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, float j, int k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilllliilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, int j, int k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilllllilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, int k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillllll(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillllllldilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, long k, long l, double m, int n, long o, long p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillilllllllfilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, long k, long l, float m, int n, long o, long p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillllllliilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, long k, long l, int m, int n, long o, long p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillillllllllilll(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, long k, long l, long m, int n, long o, long p, long q);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilll(int a, int b, long c, long d, long e);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllffffflfl(int a, int b, long c, long d, long e, float f, float g, float h, float i, float j, long k, float l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllfffffll(int a, int b, long c, long d, long e, float f, float g, float h, float i, float j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllfl(int a, int b, long c, long d, long e, float f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllfll(int a, int b, long c, long d, long e, float f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillli(int a, int b, long c, long d, long e, int f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllifl(int a, int b, long c, long d, long e, int f, float g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllii(int a, int b, long c, long d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliifl(int a, int b, long c, long d, long e, int f, int g, float h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliii(int a, int b, long c, long d, long e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliiiflll(int a, int b, long c, long d, long e, int f, int g, int h, float i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliiiiii(int a, int b, long c, long d, long e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliiiiil(int a, int b, long c, long d, long e, int f, int g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliiiill(int a, int b, long c, long d, long e, int f, int g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliiil(int a, int b, long c, long d, long e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliiilil(int a, int b, long c, long d, long e, int f, int g, int h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliiill(int a, int b, long c, long d, long e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliil(int a, int b, long c, long d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliill(int a, int b, long c, long d, long e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliillililil(int a, int b, long c, long d, long e, int f, int g, long h, long i, int j, long k, int l, long m, int n, long o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliillll(int a, int b, long c, long d, long e, int f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllil(int a, int b, long c, long d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllilffllll(int a, int b, long c, long d, long e, int f, long g, float h, float i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillliliill(int a, int b, long c, long d, long e, int f, long g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllilil(int a, int b, long c, long d, long e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllililiiliil(int a, int b, long c, long d, long e, int f, long g, int h, long i, int j, int k, long l, int m, int n, long o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllililil(int a, int b, long c, long d, long e, int f, long g, int h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllill(int a, int b, long c, long d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllillilll(int a, int b, long c, long d, long e, int f, long g, long h, int i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllilll(int a, int b, long c, long d, long e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllillll(int a, int b, long c, long d, long e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillll(int a, int b, long c, long d, long e, long f);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllldlil(int a, int b, long c, long d, long e, long f, double g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllflliiill(int a, int b, long c, long d, long e, long f, float g, long h, long i, int j, int k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllliiiii(int a, int b, long c, long d, long e, long f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllliiiil(int a, int b, long c, long d, long e, long f, int g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllliil(int a, int b, long c, long d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllil(int a, int b, long c, long d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllilill(int a, int b, long c, long d, long e, long f, int g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllill(int a, int b, long c, long d, long e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllilliiill(int a, int b, long c, long d, long e, long f, int g, long h, long i, int j, int k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllillilll(int a, int b, long c, long d, long e, long f, int g, long h, long i, int j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllilll(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllillliiill(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, int k, int l, int m, long n, long o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllilllillll(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, int k, long l, long m, long n, long o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllillll(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllli(int a, int b, long c, long d, long e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllliil(int a, int b, long c, long d, long e, long f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllil(int a, int b, long c, long d, long e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllliliil(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllilill(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllill(int a, int b, long c, long d, long e, long f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllilll(int a, int b, long c, long d, long e, long f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllillll(int a, int b, long c, long d, long e, long f, long g, int h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllll(int a, int b, long c, long d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllliiill(int a, int b, long c, long d, long e, long f, long g, long h, int i, int j, int k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllil(int a, int b, long c, long d, long e, long f, long g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllill(int a, int b, long c, long d, long e, long f, long g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllilll(int a, int b, long c, long d, long e, long f, long g, long h, int i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllillll(int a, int b, long c, long d, long e, long f, long g, long h, int i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllliiill(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, int k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllllil(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllllillll(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, long k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllllliiill(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, int k, int l, int m, long n, long o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllllillll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, int k, long l, long m, long n, long o);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllllllilll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, long l, int m, long n, long o, long p);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiilllllllllllliiill(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, long l, long m, long n, int o, int p, int q, long r, long s);
    [DllImport("icalls")]
    static internal extern int wasm_icall_iiillllllllllllillll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, long l, long m, long n, int o, long p, long q, long r, long s);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liid(int a, int b, double c);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liidddd(int a, int b, double c, double d, double e, double f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liidddddd(int a, int b, double c, double d, double e, double f, double g, double h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiddl(int a, int b, double c, double d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liidil(int a, int b, double c, int d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liidl(int a, int b, double c, long d);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiffl(int a, int b, float c, float d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liififf(int a, int b, float c, int d, float e, float f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liifil(int a, int b, float c, int d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liifill(int a, int b, float c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liifl(int a, int b, float c, long d);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiifffl(int a, int b, int c, float d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiid(int a, int b, int c, int d, double e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiidd(int a, int b, int c, int d, double e, double f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiddi(int a, int b, int c, int d, double e, double f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiif(int a, int b, int c, int d, float e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiifiiiiii(int a, int b, int c, int d, int e, int f, float g, int h, int i, int j, int k, int l, int m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiii(int a, int b, int c, int d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiiil(int a, int b, int c, int d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiiill(int a, int b, int c, int d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiilill(int a, int b, int c, int d, int e, int f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiiilll(int a, int b, int c, int d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiil(int a, int b, int c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiilil(int a, int b, int c, int d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiill(int a, int b, int c, int d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiillill(int a, int b, int c, int d, int e, long f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiilll(int a, int b, int c, int d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiilllill(int a, int b, int c, int d, int e, long f, long g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiillll(int a, int b, int c, int d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiili(int a, int b, int c, int d, long e, int f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiiliiiillliill(int a, int b, int c, int d, long e, int f, int g, int h, int i, long j, long k, long l, int m, int n, long o, long p);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiilil(int a, int b, int c, int d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiill(int a, int b, int c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiilll(int a, int b, int c, int d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiillli(int a, int b, int c, int d, long e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiili(int a, int b, int c, long d, int e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiiliiiiil(int a, int b, int c, long d, int e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilil(int a, int b, int c, long d, int e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilililil(int a, int b, int c, long d, int e, long f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiill(int a, int b, int c, long d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilli(int a, int b, int c, long d, long e, int f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilliiiiilllll(int a, int b, int c, long d, long e, int f, int g, int h, int i, int j, long k, long l, long m, long n, long o);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilliiiilllill(int a, int b, int c, long d, long e, int f, int g, int h, int i, long j, long k, long l, int m, long n, long o);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilliiiilllilll(int a, int b, int c, long d, long e, int f, int g, int h, int i, long j, long k, long l, int m, long n, long o, long p);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiillil(int a, int b, int c, long d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiillilii(int a, int b, int c, long d, long e, int f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilll(int a, int b, int c, long d, long e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiillll(int a, int b, int c, long d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiilllll(int a, int b, int c, long d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liild(int a, int b, long c, double d);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liildi(int a, int b, long c, double d, int e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liildiiff(int a, int b, long c, double d, int e, int f, float g, float h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liildl(int a, int b, long c, double d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilffffiiiii(int a, int b, long c, float d, float e, float f, float g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilffffillll(int a, int b, long c, float d, float e, float f, float g, int h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilffiiiii(int a, int b, long c, float d, float e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilffillll(int a, int b, long c, float d, float e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilfi(int a, int b, long c, float d, int e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilfifil(int a, int b, long c, float d, int e, float f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilfiiifiiii(int a, int b, long c, float d, int e, int f, int g, float h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilfiiifllll(int a, int b, long c, float d, int e, int f, int g, float h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilfl(int a, int b, long c, float d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilii(int a, int b, long c, int d, int e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliii(int a, int b, long c, int d, int e, int f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiii(int a, int b, long c, int d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiii(int a, int b, long c, int d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiiii(int a, int b, long c, int d, int e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiiiiill(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiiil(int a, int b, long c, int d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiil(int a, int b, long c, int d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiill(int a, int b, long c, int d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiil(int a, int b, long c, int d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiliiii(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiiliiiiiii(int a, int b, long c, int d, int e, int f, long g, int h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliiill(int a, int b, long c, int d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliil(int a, int b, long c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliili(int a, int b, long c, int d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliilillil(int a, int b, long c, int d, int e, long f, int g, long h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliilillll(int a, int b, long c, int d, int e, long f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliill(int a, int b, long c, int d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliilll(int a, int b, long c, int d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liiliillll(int a, int b, long c, int d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilil(int a, int b, long c, int d, long e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilili(int a, int b, long c, int d, long e, int f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililiiiii(int a, int b, long c, int d, long e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililiiill(int a, int b, long c, int d, long e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililiil(int a, int b, long c, int d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililiill(int a, int b, long c, int d, long e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililil(int a, int b, long c, int d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilill(int a, int b, long c, int d, long e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililliiiiilllll(int a, int b, long c, int d, long e, long f, int g, int h, int i, int j, int k, long l, long m, long n, long o, long p);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililliil(int a, int b, long c, int d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillil(int a, int b, long c, int d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillilllil(int a, int b, long c, int d, long e, long f, int g, long h, long i, long j, int k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillillll(int a, int b, long c, int d, long e, long f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillilllll(int a, int b, long c, int d, long e, long f, int g, long h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililll(int a, int b, long c, int d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillliil(int a, int b, long c, int d, long e, long f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililllil(int a, int b, long c, int d, long e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililllillilliiiil(int a, int b, long c, int d, long e, long f, long g, int h, long i, long j, int k, long l, long m, int n, int o, int p, int q, long r);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililllillilliiiilllll(int a, int b, long c, int d, long e, long f, long g, int h, long i, long j, int k, long l, long m, int n, int o, int p, int q, long r, long s, long t, long u, long v);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillll(int a, int b, long c, int d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililllliiliilll(int a, int b, long c, int d, long e, long f, long g, long h, int i, int j, long k, int l, int m, long n, long o, long p);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililllliililll(int a, int b, long c, int d, long e, long f, long g, long h, int i, int j, long k, int l, long m, long n, long o);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liililllll(int a, int b, long c, int d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillllli(int a, int b, long c, int d, long e, long f, long g, long h, long i, int j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilillllll(int a, int b, long c, int d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilli(int a, int b, long c, long d, int e);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillii(int a, int b, long c, long d, int e, int f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliii(int a, int b, long c, long d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiii(int a, int b, long c, long d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiiiiiillll(int a, int b, long c, long d, int e, int f, int g, int h, int i, int j, int k, long l, long m, long n, long o);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiiiiillll(int a, int b, long c, long d, int e, int f, int g, int h, int i, int j, long k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiiil(int a, int b, long c, long d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiil(int a, int b, long c, long d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiili(int a, int b, long c, long d, int e, int f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiiliiii(int a, int b, long c, long d, int e, int f, int g, long h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiill(int a, int b, long c, long d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliil(int a, int b, long c, long d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliilii(int a, int b, long c, long d, int e, int f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiliii(int a, int b, long c, long d, int e, int f, long g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliiliil(int a, int b, long c, long d, int e, int f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliilil(int a, int b, long c, long d, int e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliill(int a, int b, long c, long d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliilll(int a, int b, long c, long d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilliilllll(int a, int b, long c, long d, int e, int f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillil(int a, int b, long c, long d, int e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillili(int a, int b, long c, long d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilii(int a, int b, long c, long d, int e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilil(int a, int b, long c, long d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilill(int a, int b, long c, long d, int e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillill(int a, int b, long c, long d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilli(int a, int b, long c, long d, int e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillillil(int a, int b, long c, long d, int e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilll(int a, int b, long c, long d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilllillil(int a, int b, long c, long d, int e, long f, long g, long h, int i, long j, long k, int l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilllillll(int a, int b, long c, long d, int e, long f, long g, long h, int i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillillll(int a, int b, long c, long d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilllllillil(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, int k, long l, long m, int n, long o);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillilllllillll(int a, int b, long c, long d, int e, long f, long g, long h, long i, long j, int k, long l, long m, long n, long o);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillldil(int a, int b, long c, long d, long e, double f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllfil(int a, int b, long c, long d, long e, float f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillli(int a, int b, long c, long d, long e, int f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllii(int a, int b, long c, long d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliiii(int a, int b, long c, long d, long e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliiiiii(int a, int b, long c, long d, long e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliiil(int a, int b, long c, long d, long e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliiill(int a, int b, long c, long d, long e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliiilliiil(int a, int b, long c, long d, long e, int f, int g, int h, long i, long j, int k, int l, int m, long n);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliil(int a, int b, long c, long d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliilll(int a, int b, long c, long d, long e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllil(int a, int b, long c, long d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillliliil(int a, int b, long c, long d, long e, int f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllilil(int a, int b, long c, long d, long e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllilill(int a, int b, long c, long d, long e, int f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllililll(int a, int b, long c, long d, long e, int f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllill(int a, int b, long c, long d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllilll(int a, int b, long c, long d, long e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllillll(int a, int b, long c, long d, long e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillll(int a, int b, long c, long d, long e, long f);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllldil(int a, int b, long c, long d, long e, long f, double g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllfil(int a, int b, long c, long d, long e, long f, float g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllli(int a, int b, long c, long d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllliiil(int a, int b, long c, long d, long e, long f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllliil(int a, int b, long c, long d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllil(int a, int b, long c, long d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllilil(int a, int b, long c, long d, long e, long f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllililiil(int a, int b, long c, long d, long e, long f, int g, long h, int i, long j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllilililll(int a, int b, long c, long d, long e, long f, int g, long h, int i, long j, int k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllilill(int a, int b, long c, long d, long e, long f, int g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllill(int a, int b, long c, long d, long e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllilliillllll(int a, int b, long c, long d, long e, long f, int g, long h, long i, int j, int k, long l, long m, long n, long o, long p, long q);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllillillillllll(int a, int b, long c, long d, long e, long f, int g, long h, long i, int j, long k, long l, int m, long n, long o, long p, long q, long r, long s);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllilll(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllilllil(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, int k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllilllill(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, int k, long l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllillll(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllillllill(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, long k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllll(int a, int b, long c, long d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllfil(int a, int b, long c, long d, long e, long f, long g, float h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllli(int a, int b, long c, long d, long e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllii(int a, int b, long c, long d, long e, long f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllliiiil(int a, int b, long c, long d, long e, long f, long g, int h, int i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllliiil(int a, int b, long c, long d, long e, long f, long g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllliil(int a, int b, long c, long d, long e, long f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllil(int a, int b, long c, long d, long e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllliliiil(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllliliiliilll(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, int k, long l, int m, int n, long o, long p, long q);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllliliilliillll(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, int k, long l, long m, int n, int o, long p, long q, long r, long s);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllilil(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllililil(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, long k, int l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllililill(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, long k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllililillilllll(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, long k, int l, long m, long n, int o, long p, long q, long r, long s, long t);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllilill(int a, int b, long c, long d, long e, long f, long g, int h, long i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllill(int a, int b, long c, long d, long e, long f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllilllill(int a, int b, long c, long d, long e, long f, long g, int h, long i, long j, long k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllll(int a, int b, long c, long d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllllfil(int a, int b, long c, long d, long e, long f, long g, long h, float i, int j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllliil(int a, int b, long c, long d, long e, long f, long g, long h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllllil(int a, int b, long c, long d, long e, long f, long g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllil(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllllliliiiiil(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, long k, int l, int m, int n, int o, int p, long q);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllililil(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, long k, int l, long m, int n, long o);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllilillil(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, long k, int l, long m, long n, int o, long p);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllill(int a, int b, long c, long d, long e, long f, long g, long h, long i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liillllllllfil(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, float k, int l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllliil(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllllil(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, int l, long m);
    [DllImport("icalls")]
    static internal extern long wasm_icall_liilllllllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidd(int a, int b, double c, double d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiddd(int a, int b, double c, double d, double e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidddd(int a, int b, double c, double d, double e, double f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidddddd(int a, int b, double c, double d, double e, double f, double g, double h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiddddddd(int a, int b, double c, double d, double e, double f, double g, double h, double i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiddddddl(int a, int b, double c, double d, double e, double f, double g, double h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiddddl(int a, int b, double c, double d, double e, double f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidddl(int a, int b, double c, double d, double e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiddl(int a, int b, double c, double d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidiiii(int a, int b, double c, int d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidiil(int a, int b, double c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidl(int a, int b, double c, long d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidli(int a, int b, double c, long d, int e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viidll(int a, int b, double c, long d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viif(int a, int b, float c);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiff(int a, int b, float c, float d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifff(int a, int b, float c, float d, float e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiffffff(int a, int b, float c, float d, float e, float f, float g, float h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiffffffff(int a, int b, float c, float d, float e, float f, float g, float h, float i, float j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiffffffffl(int a, int b, float c, float d, float e, float f, float g, float h, float i, float j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifffffil(int a, int b, float c, float d, float e, float f, float g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiffffl(int a, int b, float c, float d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifffl(int a, int b, float c, float d, float e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiffl(int a, int b, float c, float d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifi(int a, int b, float c, int d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifiiii(int a, int b, float c, int d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifiil(int a, int b, float c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifiillil(int a, int b, float c, int d, int e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifil(int a, int b, float c, int d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifillil(int a, int b, float c, int d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifl(int a, int b, float c, long d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifli(int a, int b, float c, long d, int e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viifll(int a, int b, float c, long d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiid(int a, int b, int c, double d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiidd(int a, int b, int c, double d, double e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddd(int a, int b, int c, double d, double e, double f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiidddd(int a, int b, int c, double d, double e, double f, double g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiidddddd(int a, int b, int c, double d, double e, double f, double g, double h, double i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddddddl(int a, int b, int c, double d, double e, double f, double g, double h, double i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddddl(int a, int b, int c, double d, double e, double f, double g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiidddl(int a, int b, int c, double d, double e, double f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddidd(int a, int b, int c, double d, double e, int f, double g, double h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddiddl(int a, int b, int c, double d, double e, int f, double g, double h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddiiddiidl(int a, int b, int c, double d, double e, int f, int g, double h, double i, int j, int k, double l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddiiddiil(int a, int b, int c, double d, double e, int f, int g, double h, double i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddiiddiill(int a, int b, int c, double d, double e, int f, int g, double h, double i, int j, int k, long l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddiidl(int a, int b, int c, double d, double e, int f, int g, double h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddiil(int a, int b, int c, double d, double e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddiill(int a, int b, int c, double d, double e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiddl(int a, int b, int c, double d, double e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiidil(int a, int b, int c, double d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiidl(int a, int b, int c, double d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiff(int a, int b, int c, float d, float e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiifff(int a, int b, int c, float d, float e, float f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffff(int a, int b, int c, float d, float e, float f, float g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffffl(int a, int b, int c, float d, float e, float f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiifffl(int a, int b, int c, float d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiff(int a, int b, int c, float d, float e, int f, float g, float h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiffl(int a, int b, int c, float d, float e, int f, float g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiiffiifl(int a, int b, int c, float d, float e, int f, int g, float h, float i, int j, int k, float l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiiffiil(int a, int b, int c, float d, float e, int f, int g, float h, float i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiiffiill(int a, int b, int c, float d, float e, int f, int g, float h, float i, int j, int k, long l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiifl(int a, int b, int c, float d, float e, int f, int g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiil(int a, int b, int c, float d, float e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffiill(int a, int b, int c, float d, float e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiffl(int a, int b, int c, float d, float e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiifil(int a, int b, int c, float d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiifl(int a, int b, int c, float d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiflil(int a, int b, int c, float d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiid(int a, int b, int c, int d, double e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidd(int a, int b, int c, int d, double e, double f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddd(int a, int b, int c, int d, double e, double f, double g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidddd(int a, int b, int c, int d, double e, double f, double g, double h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidddddd(int a, int b, int c, int d, double e, double f, double g, double h, double i, double j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidddddddd(int a, int b, int c, int d, double e, double f, double g, double h, double i, double j, double k, double l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddddl(int a, int b, int c, int d, double e, double f, double g, double h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidddl(int a, int b, int c, int d, double e, double f, double g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddiiddiidl(int a, int b, int c, int d, double e, double f, int g, int h, double i, double j, int k, int l, double m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddiiddiil(int a, int b, int c, int d, double e, double f, int g, int h, double i, double j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddiiddiill(int a, int b, int c, int d, double e, double f, int g, int h, double i, double j, int k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddiidl(int a, int b, int c, int d, double e, double f, int g, int h, double i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddiil(int a, int b, int c, int d, double e, double f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddiill(int a, int b, int c, int d, double e, double f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiddl(int a, int b, int c, int d, double e, double f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidii(int a, int b, int c, int d, double e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidil(int a, int b, int c, int d, double e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiidl(int a, int b, int c, int d, double e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiff(int a, int b, int c, int d, float e, float f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifff(int a, int b, int c, int d, float e, float f, float g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffff(int a, int b, int c, int d, float e, float f, float g, float h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifffffffff(int a, int b, int c, int d, float e, float f, float g, float h, float i, float j, float k, float l, float m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifffffffffl(int a, int b, int c, int d, float e, float f, float g, float h, float i, float j, float k, float l, float m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffffl(int a, int b, int c, int d, float e, float f, float g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffffll(int a, int b, int c, int d, float e, float f, float g, float h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifffl(int a, int b, int c, int d, float e, float f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffiiffiifl(int a, int b, int c, int d, float e, float f, int g, int h, float i, float j, int k, int l, float m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffiiffiil(int a, int b, int c, int d, float e, float f, int g, int h, float i, float j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffiiffiill(int a, int b, int c, int d, float e, float f, int g, int h, float i, float j, int k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffiifl(int a, int b, int c, int d, float e, float f, int g, int h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffiil(int a, int b, int c, int d, float e, float f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffiill(int a, int b, int c, int d, float e, float f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiffl(int a, int b, int c, int d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifi(int a, int b, int c, int d, float e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifii(int a, int b, int c, int d, float e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifiiiiiiiiiiiill(int a, int b, int c, int d, float e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, long r, long s);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifiiiiiiiiil(int a, int b, int c, int d, float e, int f, int g, int h, int i, int j, int k, int l, int m, int n, long o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifiil(int a, int b, int c, int d, float e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifil(int a, int b, int c, int d, float e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiifl(int a, int b, int c, int d, float e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiid(int a, int b, int c, int d, int e, double f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiidddd(int a, int b, int c, int d, int e, double f, double g, double h, double i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiidddddd(int a, int b, int c, int d, int e, double f, double g, double h, double i, double j, double k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiidddddddd(int a, int b, int c, int d, int e, double f, double g, double h, double i, double j, double k, double l, double m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiddddl(int a, int b, int c, int d, int e, double f, double g, double h, double i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiidl(int a, int b, int c, int d, int e, double f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiif(int a, int b, int c, int d, int e, float f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiffff(int a, int b, int c, int d, int e, float f, float g, float h, float i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiifffffl(int a, int b, int c, int d, int e, float f, float g, float h, float i, float j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiffffl(int a, int b, int c, int d, int e, float f, float g, float h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiffliiiil(int a, int b, int c, int d, int e, float f, float g, long h, int i, int j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiifi(int a, int b, int c, int d, int e, float f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiifil(int a, int b, int c, int d, int e, float f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiifl(int a, int b, int c, int d, int e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiidl(int a, int b, int c, int d, int e, int f, double g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiifl(int a, int b, int c, int d, int e, int f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiidl(int a, int b, int c, int d, int e, int f, int g, double h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiifflf(int a, int b, int c, int d, int e, int f, int g, float h, float i, long j, float k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiifl(int a, int b, int c, int d, int e, int f, int g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiidl(int a, int b, int c, int d, int e, int f, int g, int h, double i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiifl(int a, int b, int c, int d, int e, int f, int g, int h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiidl(int a, int b, int c, int d, int e, int f, int g, int h, int i, double j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiifl(int a, int b, int c, int d, int e, int f, int g, int h, int i, float j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiidddd(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, double k, double l, double m, double n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiidl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, double k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiifl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, float k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiidl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, double l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiifl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, float l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiidl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, double m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiifl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, float m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiidl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, double n, long o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiifl(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, float n, long o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiiiiiiiiii(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t, int u, int v);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, long t);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, long r);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, long q);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, long o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, long n, long o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiili(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, long m, int n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiilil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, long m, int n, long o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, long l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiilil(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, long k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiilil(int a, int b, int c, int d, int e, int f, int g, int h, int i, long j, int k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiil(int a, int b, int c, int d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiill(int a, int b, int c, int d, int e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiilll(int a, int b, int c, int d, int e, int f, int g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiil(int a, int b, int c, int d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiili(int a, int b, int c, int d, int e, int f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiiliiii(int a, int b, int c, int d, int e, int f, int g, long h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiilil(int a, int b, int c, int d, int e, int f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiill(int a, int b, int c, int d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiilll(int a, int b, int c, int d, int e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiili(int a, int b, int c, int d, int e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiliifl(int a, int b, int c, int d, int e, int f, long g, int h, int i, float j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiliiil(int a, int b, int c, int d, int e, int f, long g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiliil(int a, int b, int c, int d, int e, int f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiiliill(int a, int b, int c, int d, int e, int f, long g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiilil(int a, int b, int c, int d, int e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiilill(int a, int b, int c, int d, int e, int f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiililll(int a, int b, int c, int d, int e, int f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiill(int a, int b, int c, int d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiilll(int a, int b, int c, int d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiillll(int a, int b, int c, int d, int e, int f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiil(int a, int b, int c, int d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliffifl(int a, int b, int c, int d, int e, long f, int g, float h, float i, int j, float k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliffil(int a, int b, int c, int d, int e, long f, int g, float h, float i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliffill(int a, int b, int c, int d, int e, long f, int g, float h, float i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiilii(int a, int b, int c, int d, int e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliifl(int a, int b, int c, int d, int e, long f, int g, int h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliii(int a, int b, int c, int d, int e, long f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliiil(int a, int b, int c, int d, int e, long f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliil(int a, int b, int c, int d, int e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiiliill(int a, int b, int c, int d, int e, long f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiilil(int a, int b, int c, int d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiililiiiill(int a, int b, int c, int d, int e, long f, int g, long h, int i, int j, int k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiilillllll(int a, int b, int c, int d, int e, long f, int g, long h, long i, long j, long k, long l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiililllllll(int a, int b, int c, int d, int e, long f, int g, long h, long i, long j, long k, long l, long m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiilli(int a, int b, int c, int d, int e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiillil(int a, int b, int c, int d, int e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiilll(int a, int b, int c, int d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiillll(int a, int b, int c, int d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiilllll(int a, int b, int c, int d, int e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiili(int a, int b, int c, int d, long e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilii(int a, int b, int c, int d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliifl(int a, int b, int c, int d, long e, int f, int g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliii(int a, int b, int c, int d, long e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiifl(int a, int b, int c, int d, long e, int f, int g, int h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiii(int a, int b, int c, int d, long e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiifl(int a, int b, int c, int d, long e, int f, int g, int h, int i, float j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiiif(int a, int b, int c, int d, long e, int f, int g, int h, int i, int j, float k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiiifl(int a, int b, int c, int d, long e, int f, int g, int h, int i, int j, float k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiiii(int a, int b, int c, int d, long e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiiil(int a, int b, int c, int d, long e, int f, int g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiiill(int a, int b, int c, int d, long e, int f, int g, int h, int i, int j, long k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiil(int a, int b, int c, int d, long e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiill(int a, int b, int c, int d, long e, int f, int g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiil(int a, int b, int c, int d, long e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiliif(int a, int b, int c, int d, long e, int f, int g, int h, long i, int j, int k, float l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiiliifl(int a, int b, int c, int d, long e, int f, int g, int h, long i, int j, int k, float l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliiill(int a, int b, int c, int d, long e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliil(int a, int b, int c, int d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiiliill(int a, int b, int c, int d, long e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilil(int a, int b, int c, int d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilill(int a, int b, int c, int d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiililll(int a, int b, int c, int d, long e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilldl(int a, int b, int c, int d, long e, long f, double g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiillfl(int a, int b, int c, int d, long e, long f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilli(int a, int b, int c, int d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiillii(int a, int b, int c, int d, long e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilliifl(int a, int b, int c, int d, long e, long f, int g, int h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilliii(int a, int b, int c, int d, long e, long f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilliiil(int a, int b, int c, int d, long e, long f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilliil(int a, int b, int c, int d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilliill(int a, int b, int c, int d, long e, long f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiillil(int a, int b, int c, int d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilll(int a, int b, int c, int d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiillll(int a, int b, int c, int d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilllli(int a, int b, int c, int d, long e, long f, long g, long h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiillllil(int a, int b, int c, int d, long e, long f, long g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiilllll(int a, int b, int c, int d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiillllll(int a, int b, int c, int d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiild(int a, int b, int c, long d, double e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiildil(int a, int b, int c, long d, double e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilf(int a, int b, int c, long d, float e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilfil(int a, int b, int c, long d, float e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiili(int a, int b, int c, long d, int e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiliffliliil(int a, int b, int c, long d, int e, float f, float g, long h, int i, long j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilii(int a, int b, int c, long d, int e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiliii(int a, int b, int c, long d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiliiiiiiiiifl(int a, int b, int c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, float n, long o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiliiiil(int a, int b, int c, long d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiliiil(int a, int b, int c, long d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiliil(int a, int b, int c, long d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiiliill(int a, int b, int c, long d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilil(int a, int b, int c, long d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilili(int a, int b, int c, long d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiililill(int a, int b, int c, long d, int e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilill(int a, int b, int c, long d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiililll(int a, int b, int c, long d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilillliliil(int a, int b, int c, long d, int e, long f, long g, long h, int i, long j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilillll(int a, int b, int c, long d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilldl(int a, int b, int c, long d, long e, double f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillfl(int a, int b, int c, long d, long e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillii(int a, int b, int c, long d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilliii(int a, int b, int c, long d, long e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilliiii(int a, int b, int c, long d, long e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilliiil(int a, int b, int c, long d, long e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilliil(int a, int b, int c, long d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillil(int a, int b, int c, long d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillili(int a, int b, int c, long d, long e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillilil(int a, int b, int c, long d, long e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillill(int a, int b, int c, long d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillli(int a, int b, int c, long d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilllil(int a, int b, int c, long d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillll(int a, int b, int c, long d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilllli(int a, int b, int c, long d, long e, long f, long g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillllil(int a, int b, int c, long d, long e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilllll(int a, int b, int c, long d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiillllll(int a, int b, int c, long d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiilllllll(int a, int b, int c, long d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viild(int a, int b, long c, double d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viildd(int a, int b, long c, double d, double e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilddd(int a, int b, long c, double d, double e, double f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viildddd(int a, int b, long c, double d, double e, double f, double g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilddddd(int a, int b, long c, double d, double e, double f, double g, double h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viildddddd(int a, int b, long c, double d, double e, double f, double g, double h, double i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilddl(int a, int b, long c, double d, double e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viildi(int a, int b, long c, double d, int e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viildid(int a, int b, long c, double d, int e, double f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viildl(int a, int b, long c, double d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilf(int a, int b, long c, float d);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilff(int a, int b, long c, float d, float e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilfff(int a, int b, long c, float d, float e, float f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilffffffl(int a, int b, long c, float d, float e, float f, float g, float h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilfffl(int a, int b, long c, float d, float e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilffil(int a, int b, long c, float d, float e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilffl(int a, int b, long c, float d, float e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilfil(int a, int b, long c, float d, int e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilfl(int a, int b, long c, float d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilfliffffiiil(int a, int b, long c, float d, long e, int f, float g, float h, float i, float j, int k, int l, int m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilfliil(int a, int b, long c, float d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilidil(int a, int b, long c, int d, double e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilidl(int a, int b, long c, int d, double e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliffff(int a, int b, long c, int d, float e, float f, float g, float h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilifffffffff(int a, int b, long c, int d, float e, float f, float g, float h, float i, float j, float k, float l, float m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilifffffffffl(int a, int b, long c, int d, float e, float f, float g, float h, float i, float j, float k, float l, float m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliffffiiii(int a, int b, long c, int d, float e, float f, float g, float h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliffffllll(int a, int b, long c, int d, float e, float f, float g, float h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilifffl(int a, int b, long c, int d, float e, float f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliffiiii(int a, int b, long c, int d, float e, float f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliffllll(int a, int b, long c, int d, float e, float f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilifil(int a, int b, long c, int d, float e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilifl(int a, int b, long c, int d, float e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilifll(int a, int b, long c, int d, float e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliflll(int a, int b, long c, int d, float e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilii(int a, int b, long c, int d, int e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliidddl(int a, int b, long c, int d, int e, double f, double g, double h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliidl(int a, int b, long c, int d, int e, double f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliifffl(int a, int b, long c, int d, int e, float f, float g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliifiil(int a, int b, long c, int d, int e, float f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliifl(int a, int b, long c, int d, int e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliii(int a, int b, long c, int d, int e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiffli(int a, int b, long c, int d, int e, int f, float g, float h, long i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiii(int a, int b, long c, int d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiii(int a, int b, long c, int d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiii(int a, int b, long c, int d, int e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiiiii(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiiiiiiiiiiil(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p, int q, long r);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiiiiiiil(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiiiiiil(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, int k, int l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiiiil(int a, int b, long c, int d, int e, int f, int g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiiil(int a, int b, long c, int d, int e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiiill(int a, int b, long c, int d, int e, int f, int g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiil(int a, int b, long c, int d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiill(int a, int b, long c, int d, int e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiiilll(int a, int b, long c, int d, int e, int f, int g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiil(int a, int b, long c, int d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiili(int a, int b, long c, int d, int e, int f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiilii(int a, int b, long c, int d, int e, int f, int g, long h, int i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiilil(int a, int b, long c, int d, int e, int f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiililill(int a, int b, long c, int d, int e, int f, int g, long h, int i, long j, int k, long l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiiill(int a, int b, long c, int d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiil(int a, int b, long c, int d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiili(int a, int b, long c, int d, int e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiill(int a, int b, long c, int d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiilll(int a, int b, long c, int d, int e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiillli(int a, int b, long c, int d, int e, int f, long g, long h, long i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliili(int a, int b, long c, int d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliilii(int a, int b, long c, int d, int e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliiliil(int a, int b, long c, int d, int e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliililil(int a, int b, long c, int d, int e, long f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliill(int a, int b, long c, int d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliilliil(int a, int b, long c, int d, int e, long f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliillil(int a, int b, long c, int d, int e, long f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliillilll(int a, int b, long c, int d, int e, long f, long g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliilll(int a, int b, long c, int d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliillll(int a, int b, long c, int d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viiliilllll(int a, int b, long c, int d, int e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilil(int a, int b, long c, int d, long e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilili(int a, int b, long c, int d, long e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililii(int a, int b, long c, int d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiii(int a, int b, long c, int d, long e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiiiil(int a, int b, long c, int d, long e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiiil(int a, int b, long c, int d, long e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiiililill(int a, int b, long c, int d, long e, int f, int g, int h, long i, int j, long k, int l, long m, long n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiiill(int a, int b, long c, int d, long e, int f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiiilll(int a, int b, long c, int d, long e, int f, int g, int h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiil(int a, int b, long c, int d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiiliil(int a, int b, long c, int d, long e, int f, int g, long h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiilill(int a, int b, long c, int d, long e, int f, int g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiill(int a, int b, long c, int d, long e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililiilll(int a, int b, long c, int d, long e, int f, int g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililil(int a, int b, long c, int d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililili(int a, int b, long c, int d, long e, int f, long g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilililii(int a, int b, long c, int d, long e, int f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilililiil(int a, int b, long c, int d, long e, int f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilililil(int a, int b, long c, int d, long e, int f, long g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililill(int a, int b, long c, int d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilililll(int a, int b, long c, int d, long e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililli(int a, int b, long c, int d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilillii(int a, int b, long c, int d, long e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililliii(int a, int b, long c, int d, long e, long f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililliil(int a, int b, long c, int d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilillil(int a, int b, long c, int d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilillili(int a, int b, long c, int d, long e, long f, int g, long h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilillill(int a, int b, long c, int d, long e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilillliiiiiiii(int a, int b, long c, int d, long e, long f, long g, int h, int i, int j, int k, int l, int m, int n, int o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilillll(int a, int b, long c, int d, long e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililllll(int a, int b, long c, int d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viililllllll(int a, int b, long c, int d, long e, long f, long g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilldd(int a, int b, long c, long d, double e, double f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilldddd(int a, int b, long c, long d, double e, double f, double g, double h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillddddi(int a, int b, long c, long d, double e, double f, double g, double h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillddddidd(int a, int b, long c, long d, double e, double f, double g, double h, int i, double j, double k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillddl(int a, int b, long c, long d, double e, double f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilldi(int a, int b, long c, long d, double e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilldidd(int a, int b, long c, long d, double e, int f, double g, double h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillffffffl(int a, int b, long c, long d, float e, float f, float g, float h, float i, float j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillffffffll(int a, int b, long c, long d, float e, float f, float g, float h, float i, float j, long k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillffffl(int a, int b, long c, long d, float e, float f, float g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillffffll(int a, int b, long c, long d, float e, float f, float g, float h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillfl(int a, int b, long c, long d, float e, long f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillfll(int a, int b, long c, long d, float e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilli(int a, int b, long c, long d, int e);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliffl(int a, int b, long c, long d, int e, float f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillii(int a, int b, long c, long d, int e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliii(int a, int b, long c, long d, int e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiidl(int a, int b, long c, long d, int e, int f, int g, double h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiffffi(int a, int b, long c, long d, int e, int f, int g, float h, float i, float j, float k, int l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiffffiiffi(int a, int b, long c, long d, int e, int f, int g, float h, float i, float j, float k, int l, int m, float n, float o, int p);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiffffiilli(int a, int b, long c, long d, int e, int f, int g, float h, float i, float j, float k, int l, int m, long n, long o, int p);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiffi(int a, int b, long c, long d, int e, int f, int g, float h, float i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiifl(int a, int b, long c, long d, int e, int f, int g, float h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiii(int a, int b, long c, long d, int e, int f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiii(int a, int b, long c, long d, int e, int f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiiii(int a, int b, long c, long d, int e, int f, int g, int h, int i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiiiii(int a, int b, long c, long d, int e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiiiiiiii(int a, int b, long c, long d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiiiiliiiiiiiiil(int a, int b, long c, long d, int e, int f, int g, int h, int i, int j, long k, int l, int m, int n, int o, int p, int q, int r, int s, int t, long u);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiil(int a, int b, long c, long d, int e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiiliil(int a, int b, long c, long d, int e, int f, int g, int h, long i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiil(int a, int b, long c, long d, int e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiliiiii(int a, int b, long c, long d, int e, int f, int g, long h, int i, int j, int k, int l, int m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiliiiiii(int a, int b, long c, long d, int e, int f, int g, long h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiliiiiiii(int a, int b, long c, long d, int e, int f, int g, long h, int i, int j, int k, int l, int m, int n, int o);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiliiiiiiii(int a, int b, long c, long d, int e, int f, int g, long h, int i, int j, int k, int l, int m, int n, int o, int p);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiliiil(int a, int b, long c, long d, int e, int f, int g, long h, int i, int j, int k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiiliil(int a, int b, long c, long d, int e, int f, int g, long h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiill(int a, int b, long c, long d, int e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliil(int a, int b, long c, long d, int e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliilii(int a, int b, long c, long d, int e, int f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiliiii(int a, int b, long c, long d, int e, int f, long g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiliiiii(int a, int b, long c, long d, int e, int f, long g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliiliil(int a, int b, long c, long d, int e, int f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliilili(int a, int b, long c, long d, int e, int f, long g, int h, long i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliilill(int a, int b, long c, long d, int e, int f, long g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliill(int a, int b, long c, long d, int e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliilllil(int a, int b, long c, long d, int e, int f, long g, long h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillili(int a, int b, long c, long d, int e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillilii(int a, int b, long c, long d, int e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliliilil(int a, int b, long c, long d, int e, long f, int g, int h, long i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilliliill(int a, int b, long c, long d, int e, long f, int g, int h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillilil(int a, int b, long c, long d, int e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillililill(int a, int b, long c, long d, int e, long f, int g, long h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillilill(int a, int b, long c, long d, int e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillill(int a, int b, long c, long d, int e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillilliill(int a, int b, long c, long d, int e, long f, long g, int h, int i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillilll(int a, int b, long c, long d, int e, long f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillillll(int a, int b, long c, long d, int e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllffffffl(int a, int b, long c, long d, long e, float f, float g, float h, float i, float j, float k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllffffffll(int a, int b, long c, long d, long e, float f, float g, float h, float i, float j, float k, long l, long m);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllffffl(int a, int b, long c, long d, long e, float f, float g, float h, float i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllffffll(int a, int b, long c, long d, long e, float f, float g, float h, float i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllffl(int a, int b, long c, long d, long e, float f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllffll(int a, int b, long c, long d, long e, float f, float g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllfl(int a, int b, long c, long d, long e, float f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllfll(int a, int b, long c, long d, long e, float f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillli(int a, int b, long c, long d, long e, int f);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliff(int a, int b, long c, long d, long e, int f, float g, float h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllii(int a, int b, long c, long d, long e, int f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliiifll(int a, int b, long c, long d, long e, int f, int g, int h, float i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliiiiii(int a, int b, long c, long d, long e, int f, int g, int h, int i, int j, int k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliiiiiii(int a, int b, long c, long d, long e, int f, int g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliiiiiiiii(int a, int b, long c, long d, long e, int f, int g, int h, int i, int j, int k, int l, int m, int n);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliiiil(int a, int b, long c, long d, long e, int f, int g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliiil(int a, int b, long c, long d, long e, int f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliil(int a, int b, long c, long d, long e, int f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliill(int a, int b, long c, long d, long e, int f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillliillll(int a, int b, long c, long d, long e, int f, int g, long h, long i, long j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllil(int a, int b, long c, long d, long e, int f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllill(int a, int b, long c, long d, long e, int f, long g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllilll(int a, int b, long c, long d, long e, int f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllldl(int a, int b, long c, long d, long e, long f, double g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllfl(int a, int b, long c, long d, long e, long f, float g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllli(int a, int b, long c, long d, long e, long f, int g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllii(int a, int b, long c, long d, long e, long f, int g, int h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllliii(int a, int b, long c, long d, long e, long f, int g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllliiiiii(int a, int b, long c, long d, long e, long f, int g, int h, int i, int j, int k, int l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllliil(int a, int b, long c, long d, long e, long f, int g, int h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllliili(int a, int b, long c, long d, long e, long f, int g, int h, long i, int j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllil(int a, int b, long c, long d, long e, long f, int g, long h);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllilil(int a, int b, long c, long d, long e, long f, int g, long h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllill(int a, int b, long c, long d, long e, long f, int g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllilllll(int a, int b, long c, long d, long e, long f, int g, long h, long i, long j, long k, long l);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllll(int a, int b, long c, long d, long e, long f, long g);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllllii(int a, int b, long c, long d, long e, long f, long g, int h, int i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllliiil(int a, int b, long c, long d, long e, long f, long g, int h, int i, int j, long k);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllliil(int a, int b, long c, long d, long e, long f, long g, int h, int i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viillllllll(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j);
    [DllImport("icalls")]
    static internal extern void wasm_icall_viilllllllllllliiil(int a, int b, long c, long d, long e, long f, long g, long h, long i, long j, long k, long l, long m, long n, int o, int p, int q, long r);
}
