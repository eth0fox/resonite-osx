using System.Runtime.InteropServices;

using Veldrid.MetalBindings;
namespace ResoniteOSXRunner;

public struct NSData
{
    public readonly IntPtr NativePtr;
    
    public NSData(IntPtr ptr) {
        if (ptr == IntPtr.Zero) {
            throw new ArgumentException("Passed null pointer");
        }
        NativePtr = ptr;
    }

    public static implicit operator IntPtr(NSData nss) => nss.NativePtr;

    public static NSData with(byte[] buffer) {
        unsafe {
            fixed (byte* pBuffer = buffer) {
                var dataPtr = ObjectiveCRuntime.IntPtr_objc_msgSend(s_class, sel_dataWithBytes_length, pBuffer, (nuint)buffer.Length);
                return new NSData(dataPtr);
            }
        }
    }
    
    public byte[] getBytes(UIntPtr length) {
        byte[] buffer = new byte[length];
        unsafe {
            fixed (byte* pBuffer = buffer) {
                ObjectiveCRuntime.objc_msgSend(NativePtr, sel_getBytes_length, pBuffer, length);
            }
        }

        return buffer;
    }
    public byte[] getBytes() {
        return getBytes(length);
    }



    public UIntPtr length => ObjectiveCRuntime.UIntPtr_objc_msgSend(NativePtr, sel_length);
    private static ObjCClass s_class = new ("NSData");
    private static readonly Selector sel_length = "length";
    private static readonly Selector sel_getBytes_length = "getBytes:length:";
    private static readonly Selector sel_dataWithBytes_length = "dataWithBytes:length:";
}