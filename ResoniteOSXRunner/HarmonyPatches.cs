
using Elements.Assets;
using FrooxEngine;
using HarmonyLib;
using Renderite.Shared;

namespace ResoniteOSXRunner;
public class HarmonyPatches {
    public static void Patch () {
        Harmony harmony = new Harmony("resoniteosxrunner");
        harmony.PatchAll();
    }
}


// By default, Resonite will generate a random character suffix for the SHM path
// this makes debugging annoying and also can cause issues with macOS' semaphore name file length limit.
[HarmonyPatch(typeof (RenderiteMessagingHost), nameof(RenderiteMessagingHost.GenerateQueueName))]
class NoRandomSHMNamePatch {
    static bool Prefix (ref string __result) {
        __result = Engine.Current.SharedMemoryPrefix;
        return false;
    }
}



// FrooxEngine assumes that if we want to start the renderer, we must be started through the bootstrapper.
// When Resonite tries to start the renderer, it will try to do so through the bootstrapper
// ...which we're not running with
[HarmonyPatch(typeof(Engine), "IsRunningWithBootstrapper", MethodType.Getter)]
class IsRunningWithBootstrapperPatch {
    static bool Prefix(ref bool __result) {
        __result = false;
        return false;
    }
}


// By default, if the OS platform is set to 'OSX' it will throw an exception on StaticShader construct (because there's
// no Metal shaders), so we just lie and say we want the DX11 shaders.
[HarmonyPatch(typeof(Shader), "ShaderPlatform", MethodType.Getter)]
class ShaderPlatformPatch {
    static bool Prefix(ref ShaderTargetPlatform __result) {
        __result = ShaderTargetPlatform.WindowsDX11;
        return false;
    }
}