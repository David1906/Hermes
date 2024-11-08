using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using Hermes.Language;
using Hermes.Models;
using Material.Icons;
using SukiUI.Toasts;
using System.Reflection;
using System.Threading.Tasks;
using Velopack;

namespace Hermes.Features.About;

public partial class AboutViewModel(
    Settings settings,
    ISukiToastManager toastManager)
    : PageBase(
        Resources.txt_about,
        MaterialIconKind.InfoOutline,
        100)
{
    public string Version { get; set; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";

    [RelayCommand]
    private async Task CheckForUpdates()
    {
        var mgr = new UpdateManager(settings.UpdateManagerUrl);
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            this.ShowSuccessToast(Resources.msg_no_updates_available);
            return;
        }

        toastManager.CreateToast()
            .OfType(NotificationType.Warning)
            .WithTitle(Resources.txt_update_available)
            .WithContent(Resources.msg_update_available + " " + newVersion.TargetFullRelease.Version.ToString())
            .WithActionButtonNormal(Resources.txt_later, _ => { }, true)
            .WithActionButton(Resources.txt_update, _ => Task.Run(async () =>
            {
                await mgr.DownloadUpdatesAsync(newVersion);
                mgr.ApplyUpdatesAndRestart(newVersion);
            }), true)
            .Queue();
    }
}