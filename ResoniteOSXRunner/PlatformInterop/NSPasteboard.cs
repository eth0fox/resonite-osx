using System.Runtime.InteropServices;

using Veldrid.MetalBindings;
namespace ResoniteOSXRunner;

public struct NSPasteboard {
    public static NSString NSPasteboardTypeString = NSString.New("public.utf8-plain-text");
    public static NSString NSPasteboardTypePng = NSString.New("public.png");
    public static NSString NSPasteboardTypeFileUrl = NSString.New("public.file-url");
    public static NSString NSPasteboardURLReadingFileURLsOnlyKey =
        NSString.New("NSPasteboardURLReadingFileURLsOnlyKey");
    
    public readonly IntPtr NativePtr;

    public NSPasteboard(IntPtr ptr) {
        if (ptr == IntPtr.Zero) {
            throw new ArgumentException("Passed null pointer");
        }
        NativePtr = ptr;
    }

    public static implicit operator IntPtr(NSPasteboard nss) => nss.NativePtr;

    public bool hasAvailableType(NSArray types) {
        return ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_availableTypeFromArray, types) != IntPtr.Zero;
    }
    
    
    
    public NSString? stringForType(NSString type) {
        var handle = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_stringForType, type);
        if (handle == IntPtr.Zero) return null;
        return new NSString(handle);
    }

    
    public bool setStringForType(NSString type, string str) {
        var nsstring = NSString.New(str);
        return ObjectiveCRuntime.bool_objc_msgSend(NativePtr, sel_setString_forType, nsstring, type);
    }
    
    public nint clearContents() {
        return ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_clearContents);
    }

    public NSArray? readObjectsForClasses(NSArray classes, NSDictionary options) {
        var handle = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_readObjectsForClasses_options, classes, options);
        if (handle == IntPtr.Zero) return null;
        return new NSArray(handle); 
    }
    
    public NSArray? pasteboardItems {
        get {
            var handle = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_pasteboardItems);
            if (handle == IntPtr.Zero) return null;
            return new NSArray(handle);
        }
    }

    public static NSPasteboard generalPasteboard => new (ObjectiveCRuntime.IntPtr_objc_msgSend(s_class, sel_generalPasteboard));
    private static ObjCClass s_class = new ("NSPasteboard");
    private static readonly Selector sel_generalPasteboard = "generalPasteboard";
    private static readonly Selector sel_availableTypeFromArray= "availableTypeFromArray:";
    private static readonly Selector sel_stringForType= "stringForType:";
    private static readonly Selector sel_setString_forType= "setString:forType:";
    private static readonly Selector sel_clearContents= "clearContents";
    private static readonly Selector sel_pasteboardItems= "pasteboardItems";
    private static readonly Selector sel_readObjectsForClasses_options= "readObjectsForClasses:options:";
}


public struct NSPasteboardItem {
    
    public readonly IntPtr NativePtr;

    public NSPasteboardItem(IntPtr ptr) {
        if (ptr == IntPtr.Zero) {
            throw new ArgumentException("Passed null pointer");
        }
        NativePtr = ptr;
    }

    public static implicit operator IntPtr(NSPasteboardItem nss) => nss.NativePtr;
    
    
    public bool hasAvailableType(NSArray types) {
        return ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_availableTypeFromArray, types) != IntPtr.Zero;
    }

    
    public NSString? stringForType(NSString type) {
        var handle = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_stringForType, type);
        if (handle == IntPtr.Zero) return null;
        return new NSString(handle);
    }
    
    private static ObjCClass s_class = new ("NSPasteboardItem");
    private static readonly Selector sel_stringForType= "stringForType:";
    private static readonly Selector sel_availableTypeFromArray= "availableTypeFromArray:";
}
