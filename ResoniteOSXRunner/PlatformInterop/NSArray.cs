using System.Runtime.InteropServices;

using Veldrid.MetalBindings;
namespace ResoniteOSXRunner;

public struct NSArray
{
    public readonly IntPtr NativePtr;
    
    public NSArray(IntPtr ptr) => NativePtr = ptr;
    
    public static implicit operator IntPtr(NSArray nss) => nss.NativePtr;




    
    public static NSArray with(IntPtr[] objects) {
        return new NSArray(ObjectiveCRuntime.IntPtr_objc_msgSend(s_class,  sel_arrayWithObjects_count, objects, (UIntPtr)objects.Length));
    }


    public IntPtr this[UIntPtr i] {
        get => ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_objectAtIndex, i);
    }
    public UIntPtr count => ObjectiveCRuntime.UIntPtr_objc_msgSend(NativePtr, sel_count);
    private static ObjCClass s_class = new ("NSArray");
    private static readonly Selector sel_count = "count";
    private static readonly Selector sel_arrayWithObjects_count = "arrayWithObjects:count:";
    private static readonly Selector sel_objectAtIndex = "objectAtIndex:";
}