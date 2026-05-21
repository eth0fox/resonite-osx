
using System.Diagnostics;
using FrooxEngine;
using HarmonyLib;
using Renderite.Shared;

namespace ResoniteOSXRunner;


public class OSXEngineRunner {
    public static async Task RunFrooxEngine (string[] args) {
        

        var launchOptions = LaunchOptions.GetLaunchOptions(args);
        launchOptions.OutputDevice = HeadOutputDevice.Screen;
        launchOptions.DataDirectory ??= StandaloneFrooxEngineRunner.DefaultDataDirectory;
        launchOptions.CacheDirectory ??= StandaloneFrooxEngineRunner.DefaultCacheDirectory;

        
        var systemInfo = new OSXSystemInfo();
        var engine = new Engine();
        Console.WriteLine("Initializing FrooxEngine...");
        
        var shutdownComplete = false;
        engine.EnvironmentShutdownCallback = () => shutdownComplete = true;
        engine.EnvironmentCrashCallback = () => {
            Console.Error.WriteLine("EnvironmentCrashCallback called! Exiting immediately");
            Process.GetCurrentProcess().Kill();
        };
        
        
        
        await engine.Initialize(
            StandaloneFrooxEngineRunner.AssemblyDirectory,
            true,
            launchOptions,
            systemInfo,
            new ConsoleEngineInitProgress()
        );
        engine.InputInterface.RegisterClipboardInterface(new OSXClipboardInterface());
        
        Console.WriteLine("Configuring Userspace...");
        Userspace.SetupUserspace(engine);
        
        
        Console.WriteLine("Entering Update Loop...");
        
        
        var ShutdownRequested = false;
        Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) => {
            if (ShutdownRequested) {
                Console.WriteLine("Second CTRL+C press detected, Exiting immediately");
                Process.GetCurrentProcess().Kill();
            } else {
                Console.WriteLine("Requesting shutdown...");
                e.Cancel = true;
                ShutdownRequested = true;
            }
        };
        var updateLoop = new Thread(
            () => {
                var stopwatch = Stopwatch.StartNew();
                var frameBudget = Stopwatch.Frequency / 60;
                var tickMS = Stopwatch.Frequency / 1000;
                long ldt = 0;
                while (!shutdownComplete) {
                    engine.RunUpdateLoop();
                    systemInfo.FrameFinished();
                    // engine.PerfStats.Update(systemInfo);
                    if (ShutdownRequested)
                        Userspace.ExitApp(false);
                    // var time = stopwatch.ElapsedTicks - ldt;
                    // stopwatch.Restart();
                    // var delay =  frameBudget - time;
                    // if (delay <= 0) {
                    //     Console.WriteLine($"Last frame was over budget by {-delay} ticks! (budget {frameBudget}, ~{-delay / tickMS}ms)");
                    //     ldt = 0;
                    // }
                    // else {
                    //     ldt = delay;
                    //     var ms = (int)(ldt / tickMS);
                    //     //Thread.Sleep(ms);
                    // }
                }

            });
        updateLoop.Name = "FrooxEngine Update Loop";
        updateLoop.Priority = ThreadPriority.Normal;
        updateLoop.IsBackground = false;
        updateLoop.Start();
        
        updateLoop.Join();
        engine.Dispose();

    }
}
