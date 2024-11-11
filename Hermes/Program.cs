using Avalonia;
using System;
using System.Threading;
using Avalonia.Controls;
using Hermes.Common;
using Velopack;

namespace Hermes;

sealed class Program
{
    static Mutex mutex = new(false, "hermes.ingrasys");

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (!mutex.WaitOne(TimeSpan.FromSeconds(2), false))
        {
            Console.WriteLine("Another instance of the app is running. Bye!");
            NativeMethods.PostMessage(
                (IntPtr)NativeMethods.HWND_BROADCAST,
                NativeMethods.WM_SHOWME,
                IntPtr.Zero,
                IntPtr.Zero);
            return;
        }

        try
        {
            VelopackApp.Build().Run();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}