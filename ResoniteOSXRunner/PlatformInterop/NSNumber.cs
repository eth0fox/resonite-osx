using System.Runtime.InteropServices;

using Veldrid.MetalBindings;
namespace ResoniteOSXRunner;

public struct NSNumber
{
    public readonly IntPtr NativePtr;
    
    public NSNumber(IntPtr ptr) => NativePtr = ptr;
    
    public static implicit operator IntPtr(NSNumber nss) => nss.NativePtr;

    public static NSNumber False =>
        new NSNumber(ObjectiveCRuntime.IntPtr_objc_msgSend(s_class, sel_numberWithBool, (uint)0));
    public static NSNumber True =>
        new NSNumber(ObjectiveCRuntime.IntPtr_objc_msgSend(s_class, sel_numberWithBool, (uint)1));
    
    private static ObjCClass s_class = new ("NSNumber");
    private static readonly Selector sel_numberWithBool = "numberWithBool:";
}