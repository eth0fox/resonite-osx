using System.Runtime.InteropServices;
using Veldrid.MetalBindings;

namespace ResoniteOSXRunner;

public static class ObjectiveCRuntimeExtns {
    private const string ObjCLibrary = "/usr/lib/libobjc.A.dylib";
    extension(Veldrid.MetalBindings.ObjectiveCRuntime) {
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr IntPtr_objc_msgSend(
            IntPtr target,
            Selector selector,
            [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 3)]
            IntPtr[] arg1,
            UIntPtr length
        );

        
        [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
        public static extern IntPtr IntPtr_objc_msgSend(
            IntPtr target,
            Selector selector,
            [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 4)]
            IntPtr[] arg1,
            [MarshalAs(UnmanagedType.LPArray, IidParameterIndex = 4)]
            IntPtr[] arg2,
            UIntPtr length
        );

        
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern bool bool_objc_msgSend(IntPtr receiver, Selector selector, IntPtr a, IntPtr b);
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, Selector selector, IntPtr a, IntPtr b);
        [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
        unsafe public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, Selector selector, void* a, UIntPtr b);
    }
}
