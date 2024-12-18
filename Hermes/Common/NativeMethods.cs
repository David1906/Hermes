using System.Runtime.InteropServices;
using System;

namespace Hermes.Common;

public class NativeMethods
{
    public const int HWND_BROADCAST = 0xffff;
    public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

    [DllImport("user32")]
    public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32")]
    public static extern int RegisterWindowMessage(string message);
}