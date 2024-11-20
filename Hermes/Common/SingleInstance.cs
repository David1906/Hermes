using System;
using System.Diagnostics;

namespace Hermes.Common;

public sealed class SingleInstance
{
    public static bool ExistsAnotherInstance()
    {
        try
        {
            Process currentProcess = Process.GetCurrentProcess();
            foreach (var p in Process.GetProcesses())
            {
                if (p.Id != currentProcess.Id)
                {
                    if (p.ProcessName.Equals(currentProcess.ProcessName))
                    {
                        IntPtr hFound = p.MainWindowHandle;
                        if (User32API.IsIconic(hFound))
                        {
                            User32API.ShowWindow(hFound, User32API.SW_RESTORE);
                        }

                        User32API.SetForegroundWindow(hFound);
                        return true;
                    }
                }
            }
        }
        catch
        {
        }

        return false;
    }

    internal static bool IsAlreadyRunning(string processName)
    {
        foreach (var p in Process.GetProcesses())
        {
            if (p.ProcessName.Equals(processName))
            {
                return true;
            }
        }

        return false;
    }
}