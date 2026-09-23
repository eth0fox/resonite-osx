
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
        engine.EnvironmentShutdownCallback = () => {
            shutdownComplete = true;
            try {
                engine.RenderSystem.ShutdownRenderer();
            }
            catch (Exception e) {
                Console.WriteLine("Failed to shutdown renderer: " + e);
            }

            Task.Delay(10000).ContinueWith(_ => {
                    Console.WriteLine("Process is still alive 10 seconds after shutdown. Forcefully exiting...");
                    Environment.Exit(0); 
                }
            );
        };
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
        
        
        // this is only used for performance metrics - we use a custom ISystemInfo for the real SystemInfo as we need
        // to override some fields that we can't do by just subclassing StandaloneSystemInfo.
        var standaloneSystemInfo = new StandaloneSystemInfo();
        var updateLoop = new Thread(
            () => {
                
                var stopwatch = Stopwatch.StartNew();
                
                engine.InitializeUpdateLoop();
                var frameBudget = Stopwatch.Frequency / 60;
                var tickMS = Stopwatch.Frequency / 1000;
                long ldt = 0;
                var frames = new long[100];
                var frameI = 0;
                while (!shutdownComplete) {
                    engine.RunUpdateLoop();
                    standaloneSystemInfo.FrameFinished();
                    engine.PerfStats.Update(standaloneSystemInfo);
                    if (ShutdownRequested)
                        Userspace.ExitApp(false);
                    var time = stopwatch.ElapsedTicks - ldt;
                    var wrapped = frameI % frames.Length;
                    frames[wrapped] = time;
                    if (wrapped == 0) {
                        Console.WriteLine("Average engine frame rate: " + (Stopwatch.Frequency / frames.Average()));
                    } 
                    frameI++;
                    stopwatch.Restart();
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
