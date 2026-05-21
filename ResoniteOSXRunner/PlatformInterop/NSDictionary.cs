using System.Runtime.InteropServices;

using Veldrid.MetalBindings;
namespace ResoniteOSXRunner;

public struct NSDictionary
{
    public readonly IntPtr NativePtr;
    
    public NSDictionary(IntPtr ptr) => NativePtr = ptr;
    
    public static implicit operator IntPtr(NSDictionary nss) => nss.NativePtr;




    public static NSDictionary with(IntPtr[] keys, IntPtr[] values) {
         if (keys.Length != values.Length)
             throw new ArgumentException("keys and values must have the same length");
        return new NSDictionary(ObjectiveCRuntime.IntPtr_objc_msgSend(s_class, sel_dictionaryWithObjects_forKeys_count, keys, values, (UIntPtr)keys.Length));
    }


    public UIntPtr count => ObjectiveCRuntime.UIntPtr_objc_msgSend(NativePtr, sel_count);
    private static ObjCClass s_class = new ("NSDictionary");
    private static readonly Selector sel_count = "count";
    private static readonly Selector sel_dictionaryWithObjects_forKeys_count = "dictionaryWithObjects:forKeys:count:";
}