using FrooxEngine;

namespace ResoniteOSXRunner;

class OSXSystemInfo : StandaloneSystemInfo {
    
    
    public float RenderTime => 0.0f;
    public OSXSystemInfo () {
        if (System.OperatingSystem.IsMacOS()) {
            Platform = Platform.OSX;
            try {
                OperatingSystem = "macOS " + Sysctl.GetSysctlString("kern.osproductversion");
                CPU = Sysctl.GetSysctlString("machdep.cpu.brand_string");
                PhysicalCores = BitConverter.ToInt32(Sysctl.GetSysctl("hw.physicalcpu_max"));
                MemoryBytes = BitConverter.ToInt64(Sysctl.GetSysctl("hw.memsize_usable"));
            }
            catch (Exception e) { }
            try {
                var displays = Sysctl.RunCommand("system_profiler", "SPDisplaysDataType").Split("\n");
                GPU = (displays.FirstOrDefault((a) => a.StartsWith("      Chipset Model: ")) ?? "      Chipset Model: UNKNOWN").Substring(21);
                XRDeviceModel = (displays.FirstOrDefault((a) => a.StartsWith("          Display Type: "))  ?? "          Display Type: UNKNOWN").Substring(24);
            } catch(Exception e) {}     
        }
        
    }
}