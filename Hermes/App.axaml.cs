using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia;
using ConfigFactory.Avalonia.Helpers;
using Hermes.Common.Extensions;
using Hermes.Common.Messages;
using Hermes.Common;
using Hermes.Features;
using Hermes.Repositories;
using Hermes.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Hermes.Features.Main;

namespace Hermes
{
    public partial class App : Application
    {
        private readonly ServiceProvider _provider;
        private readonly ILogger? _logger;
        private WindowService? _windowService;
        private Window? _mainWindow;

        public App()
        {
            _provider = this.GetServiceProvider();
            this._logger = _provider.GetService<ILogger>()!;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Dispatcher.UIThread.UnhandledException += this.OnUnhandledException;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                this._mainWindow = _provider.BuildWindow<MainWindowViewModel>(true);
                desktop.MainWindow = this._mainWindow;
                BrowserDialog.StorageProvider = desktop.MainWindow?.StorageProvider;
                this._windowService = _provider.GetRequiredService<WindowService>();
                this._windowService.Start();
                Task.Run(this.InitializeMainView);
            }

            base.OnFrameworkInitializationCompleted();
        }

        private Task InitializeMainView()
        {
            if (this._mainWindow is null) return Task.CompletedTask;

            _provider.GetRequiredService<ISettingsRepository>().Load();
            WeakReferenceMessenger.Default.Send(new SplashMessage(Language.Resources.txt_migrating_local_context));
            _provider.GetRequiredService<HermesLocalContext>().Migrate();
            WeakReferenceMessenger.Default.Send(new SplashMessage(Language.Resources.txt_migrating_remote_context));
            _provider.GetRequiredService<HermesRemoteContext>().Migrate();
            _provider.GetRequiredService<PagePrototype>().Provider = _provider;
            
            Dispatcher.UIThread.Invoke(() =>
            {
                this._mainWindow.Content = _provider.GetRequiredService<MainViewModel>();
            });
            WeakReferenceMessenger.Default.Send(new SplashClosedMessage());
            return Task.CompletedTask;
        }

        public static void Restart()
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath)) return;
            Process.Start(new ProcessStartInfo(exePath) { UseShellExecute = true });
            Environment.Exit(0);
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            const string title = "Unhandled Exception";
            this._logger?.Error($"{title}: {e.Exception.Message}");
            this._windowService?.ShowToast(this, new ShowToastMessage(title, e.Exception.Message));
            e.Handled = true;
        }

        private void NativeMenuItem_OnClick(object? sender, EventArgs e)
        {
            this._mainWindow?.Show();
            this._mainWindow?.Activate();
            if (this._mainWindow != null)
            {
                var oldTopMost = this._mainWindow.Topmost;
                this._mainWindow.Topmost = true;
                this._mainWindow.Topmost = oldTopMost;
            }
        }
    }
}