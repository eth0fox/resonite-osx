
using System.Reflection;
using System.Runtime.InteropServices;
using FrooxEngine;
using HarmonyLib;
using Renderite.Shared;
using ResoniteOSXRunner;
using Veldrid.MetalBindings;
using NSArray = ResoniteOSXRunner.NSArray;

NativeLibrary.Load("/System/Library/Frameworks/AppKit.framework/AppKit");


try {
    string FROOXENGINE_PATH = Environment.GetEnvironmentVariable("FROOXENGINE_PATH");
    string NATIVES_PATH = Environment.GetEnvironmentVariable("NATIVES_PATH");
    if (FROOXENGINE_PATH is null)
        throw new ArgumentNullException("Please set FROOXENGINE_PATH environment variable.");


    Assembly? TryAssemblyFromPath(string dir, string name) {
        var path = Path.Combine(dir, name + ".dll");
        if (!File.Exists(path)) return null;
        return Assembly.LoadFrom(path);
    }

    Assembly? TryAssembly(string name) {
        return TryAssemblyFromPath(NATIVES_PATH, name) ??
               TryAssemblyFromPath(FROOXENGINE_PATH, name);

    }

    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(
        ((sender, e) => {
            var name = new AssemblyName(e.Name).Name;
            Console.WriteLine("Try load assembly " + name);
            if (name is null) return null;
            return TryAssembly(name);
        })
    );
   

    HarmonyPatches.Patch();
    
    string[] forceLoadAssemblies = [
        "FrooxEngine.Store",
        "ProtoFlux.Core",
        "ProtoFlux.Nodes.Core",
        "ProtoFlux.Nodes.FrooxEngine",
        "ProtoFluxBindings",
        "Awwdio",
        "PhotonDust",
        "NYoutubeDL",
        "YellowDogMan.Cloudtoid.Interprocess",
        "Steamworks.NET"
    ];  
    foreach (string assem in forceLoadAssemblies) 
        if (TryAssembly(assem) is null) throw new FileNotFoundException("Couldn't load assembly " + assem);

    // FrooxEngine tries to start the renderer with a specific working directory so we need to ensure that exists
    if (!Directory.Exists("Renderer")) Directory.CreateDirectory("Renderer");
    await OSXEngineRunner.RunFrooxEngine(args);
    



}
catch (Exception e) {
    Console.WriteLine("Something went wrong. " + e.ToString());
    Environment.Exit(e.HResult);
    
}