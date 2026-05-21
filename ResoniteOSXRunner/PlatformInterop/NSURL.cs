using System.Runtime.InteropServices;

using Veldrid.MetalBindings;
namespace ResoniteOSXRunner;

public struct NSURL {
    public readonly IntPtr NativePtr;

    public NSURL(IntPtr ptr) {
        if (ptr == IntPtr.Zero) {
            throw new ArgumentException("Passed null pointer");
        }
        NativePtr = ptr;
    }

    public static implicit operator IntPtr(NSURL nss) => nss.NativePtr;
    public NSString path => new (ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_path));
    public static ObjCClass s_class = new ("NSURL");
    public static Selector sel_path = "path";
}

