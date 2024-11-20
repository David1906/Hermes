using System.Diagnostics;
using System.Threading.Tasks;
using Hermes.Common;

namespace Hermes.Services;

public class RestartOnCrashService(FileService fileService)
{
    private static readonly string AppBaseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string BasePath = @$"{AppBaseDirectory}AppData\";
    private static readonly string ExecutableFullPath = $@"{BasePath}\RestartOnCrash.exe";
    private static readonly string SettingsFullPath = $@"{BasePath}\settings.ini";
    private const string ProcessName = "RestartOnCrash";

    public async Task Start()
    {
        if (!SingleInstance.IsAlreadyRunning(ProcessName))
        {
            await Setup();
            Process.Start(ExecutableFullPath);
        }
    }

    private async Task Setup()
    {
        await fileService.WriteAllTextAsync(
            SettingsFullPath,
            @$"
                [general]
                RestartGracePeriod=5
                Autorun=1
                StartMinimized=1
                LogToFile=1
                LogFileName={BasePath}\RoC.txt
                CheckForUpdates=1
                MinimizeOnClose=1
                ManualRestartConfirmationEnabled=1

                [Application0]
                FileName={AppBaseDirectory}Hermes.exe
                WindowTitle=
                Enabled=1
                Command=""{AppBaseDirectory}Hermes.exe""
                CommandEnabled=1
                WorkingDirectory={AppBaseDirectory}
                CrashNotResponding=1
                CrashNotRunning=1
                KillIfHanged=1
                CloseProblemReporter=1
                DelayEnabled=1
                CrashDelay=1".Trim());
    }
}