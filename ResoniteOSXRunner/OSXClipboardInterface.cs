using System.Runtime.InteropServices;
using Elements.Assets;
using FrooxEngine;
using Veldrid.MetalBindings;

namespace ResoniteOSXRunner;
public class OSXClipboardInterface  : IClipboardInterface, IDisposable {
    private NSPasteboard? pasteboard;
    public OSXClipboardInterface () {
        try {
            pasteboard = NSPasteboard.generalPasteboard;
        } catch (Exception ex) {
            Console.Error.WriteLine("Failed to get NSPasteboard: " + ex.Message);
        }
    }

    
    

    private NSArray stringTypes = NSArray.with([NSPasteboard.NSPasteboardTypeString]);
    private NSArray imgType = NSArray.with([NSPasteboard.NSPasteboardTypePng]);
    private NSArray fileType = NSArray.with([NSPasteboard.NSPasteboardTypeFileUrl]);
    public bool ContainsText => pasteboard?.hasAvailableType(stringTypes) == true;
    public bool ContainsFiles => pasteboard?.hasAvailableType(fileType) == true;
    public bool ContainsImage => pasteboard?.hasAvailableType(imgType) == true;

    private NSDictionary ReadFileUrlsOnlyOptionsDictionary = NSDictionary.with(
        [NSPasteboard.NSPasteboardURLReadingFileURLsOnlyKey],
        [NSNumber.True]
    );

    private NSArray NSURLClassArray = NSArray.with([ NSURL.s_class ]);
    
    public Task<string> GetText () {
        return Task.FromResult(pasteboard?.stringForType(NSPasteboard.NSPasteboardTypeString)?.GetValue() ?? "");
    }
    public Task<List<string>> GetFiles () {
        var list = new List<string>();
        if (pasteboard is not null) {
            var items = pasteboard?.readObjectsForClasses(NSURLClassArray, ReadFileUrlsOnlyOptionsDictionary);
            var count = items?.count;

            for (UIntPtr i = 0; i < count; i++) {
                var ptr = items?[i];
                if (!ptr.HasValue || ptr == IntPtr.Zero) continue;
                var item = new NSURL(ptr.Value);
                var nsstr = item.path;
                if (nsstr.NativePtr == IntPtr.Zero) continue;
                var str = nsstr.GetValue();
                list.Add(str);
            }
            
        }
        return Task.FromResult(list);
    }
    public Task<Bitmap2D> GetImage () {
            throw new NotImplementedException();
    }
    public Task<bool> SetText(string text) {
        pasteboard?.clearContents();
        return Task.FromResult(pasteboard?.setStringForType(NSPasteboard.NSPasteboardTypeString, text) == true);
    }
    public Task<bool> SetBitmap(Bitmap2D bitmap) {
        throw new NotImplementedException();
    }

    
    public void Dispose () {}
}
