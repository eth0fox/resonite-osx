using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ResoniteOSXRunner;

class Sysctl {
    [DllImport("libSystem.dylib", SetLastError = true)]
    static extern int sysctlbyname(byte[] attrName, byte[] buf, ref nint size, int newp, int newlen);
    [DllImport("libSystem.dylib", SetLastError = true)]
    static extern int sysctlbyname(byte[] attrName, IntPtr buf, out nint size, int newp, int newlen);

    private static void CheckReturnValue(int returnValue) {
        if (returnValue == 0) return;
        var errno = Marshal.GetLastWin32Error(); // yes, that is really what that function is called
        if (errno == 1) throw new SystemException("Operation not permitted");
        if (errno == 2) throw new FileNotFoundException("ENOENT: No such file or directory");
        if (errno == 12) throw new OutOfMemoryException("ENOMEM: Cannot allocate memory");
        if (errno == 14) throw new SystemException("EFAULT: Bad address");
        if (errno == 20) throw new InvalidOperationException("ENOTDIR: Not a directory");
        if (errno == 22) throw new ArgumentException("EINVAL: Invalid argument");
        if (errno == 21) throw new InvalidOperationException("EISDIR: Is a directory");
        throw new SystemException($"Error {errno}");
    }
    public static byte[] GetSysctl(string name) {
        var nameBuf = System.Text.Encoding.ASCII.GetBytes(name + "\0");
        CheckReturnValue(sysctlbyname(nameBuf, IntPtr.Zero, out var len, 0, 0));
        var outputBuf = new byte[len];
        CheckReturnValue(sysctlbyname(nameBuf, outputBuf, ref len, 0, 0));
        return outputBuf;
    }

    public static string GetSysctlString(string name) {
        return System.Text.Encoding.ASCII.GetString(Sysctl.GetSysctl(name)).TrimEnd('\0');
    }
    public static string RunCommand(string binary, string arguments) {
        var process = new Process();
        try {
            process.StartInfo = new ProcessStartInfo(binary, arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            return process.StandardOutput.ReadToEnd();
        } finally {
            process.Dispose();
        }
    }
    
    
}
