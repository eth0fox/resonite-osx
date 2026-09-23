using System.Diagnostics;
using System.Runtime.InteropServices;
using FrooxEngine;

namespace ResoniteOSXRunner;


class OSXSystemInfo : ISystemInfo {
    
    
    public string OperatingSystem { get; }
    public string CPU { get; }
    public string GPU { get; }
    public int? PhysicalCores { get; }
    public long MemoryBytes { get; }
    public long VRAMBytes { get; }
    public string XRDeviceName { get; }
    public string XRDeviceModel { get; }
    public OSXSystemInfo () {
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

    public Platform Platform => Platform.OSX;
    public Architecture Architecture => RuntimeInformation.ProcessArchitecture;

    public string UniqueDeviceIdentifier => null;
    public bool IsAOT => false;
    public void RegisterThread(string name) {}
    public void BeginSample(string name) {}
    public void EndSample () {}

    
    
    
}