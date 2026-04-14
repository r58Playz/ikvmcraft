using System;
using System.Runtime.InteropServices;

internal unsafe static class LibJvm
{
	const string Name = "/ikvm/bin/libjvm.so";

    // Invocation API
    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int JNI_GetDefaultJavaVMInitArgs(void* vm_args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int JNI_GetCreatedJavaVMs(IntPtr* vmBuf, int bufLen, int* nVMs);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int JNI_CreateJavaVM(IntPtr* p_vm, IntPtr* p_env, void* vm_args);

    // JVM helpers
    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void JVM_ThrowException(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string message);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int JVM_ActiveProcessorCount();

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int JVM_IHashCode(IntPtr env, IntPtr handle);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void JVM_ArrayCopy(
        IntPtr env, IntPtr ignored,
        IntPtr src, int src_pos,
        IntPtr dst, int dst_pos, int length);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr JVM_InitProperties(IntPtr env, IntPtr props);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr JVM_RawMonitorCreate();

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void JVM_RawMonitorDestroy(IntPtr mon);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int JVM_RawMonitorEnter(IntPtr mon);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void JVM_RawMonitorExit(IntPtr mon);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void JVM_CopySwapMemory(
        IntPtr env,
        IntPtr srcObj, long srcOffset,
        IntPtr dstObj, long dstOffset,
        long size, long elemSize);

    // JVM loader helpers
    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void JVM_Init(IntPtr iface);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr JVM_LoadLibrary(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void JVM_UnloadLibrary(IntPtr handle);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr JVM_FindLibraryEntry(
        IntPtr handle,
        [MarshalAs(UnmanagedType.LPStr)] string name);

    // JNI method call helpers - Instance methods
    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_CallObjectMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_CallObjectMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool __JNI_CallBooleanMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool __JNI_CallBooleanMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte __JNI_CallByteMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte __JNI_CallByteMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern char __JNI_CallCharMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern char __JNI_CallCharMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern short __JNI_CallShortMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern short __JNI_CallShortMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int __JNI_CallIntMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int __JNI_CallIntMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern long __JNI_CallLongMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern long __JNI_CallLongMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern float __JNI_CallFloatMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern float __JNI_CallFloatMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern double __JNI_CallDoubleMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern double __JNI_CallDoubleMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void __JNI_CallVoidMethod(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void __JNI_CallVoidMethodV(IntPtr env, IntPtr obj, IntPtr methodID, IntPtr va_list);

    // JNI method call helpers - Nonvirtual methods
    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_CallNonvirtualObjectMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_CallNonvirtualObjectMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool __JNI_CallNonvirtualBooleanMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool __JNI_CallNonvirtualBooleanMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte __JNI_CallNonvirtualByteMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte __JNI_CallNonvirtualByteMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern char __JNI_CallNonvirtualCharMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern char __JNI_CallNonvirtualCharMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern short __JNI_CallNonvirtualShortMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern short __JNI_CallNonvirtualShortMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int __JNI_CallNonvirtualIntMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int __JNI_CallNonvirtualIntMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern long __JNI_CallNonvirtualLongMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern long __JNI_CallNonvirtualLongMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern float __JNI_CallNonvirtualFloatMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern float __JNI_CallNonvirtualFloatMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern double __JNI_CallNonvirtualDoubleMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern double __JNI_CallNonvirtualDoubleMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void __JNI_CallNonvirtualVoidMethod(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void __JNI_CallNonvirtualVoidMethodV(IntPtr env, IntPtr obj, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    // JNI method call helpers - Static methods
    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_CallStaticObjectMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_CallStaticObjectMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool __JNI_CallStaticBooleanMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool __JNI_CallStaticBooleanMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte __JNI_CallStaticByteMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte __JNI_CallStaticByteMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern char __JNI_CallStaticCharMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern char __JNI_CallStaticCharMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern short __JNI_CallStaticShortMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern short __JNI_CallStaticShortMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int __JNI_CallStaticIntMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern int __JNI_CallStaticIntMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern long __JNI_CallStaticLongMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern long __JNI_CallStaticLongMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern float __JNI_CallStaticFloatMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern float __JNI_CallStaticFloatMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern double __JNI_CallStaticDoubleMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern double __JNI_CallStaticDoubleMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void __JNI_CallStaticVoidMethod(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern void __JNI_CallStaticVoidMethodV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);

    // JNI object creation helpers
    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_NewObject(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr args);

    [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr __JNI_NewObjectV(IntPtr env, IntPtr clazz, IntPtr methodID, IntPtr va_list);
}
