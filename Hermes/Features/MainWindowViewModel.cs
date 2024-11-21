using System;
using System.Threading.Tasks;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hermes.Common.Messages;
using Hermes.Features.Login;
using Hermes.Features.Main;
using Hermes.Language;
using Hermes.Models;
using Hermes.Services;
using Hermes.Types;
using R3;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using SukiUI;

namespace Hermes.Features
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }
        public bool CanClose { get; private set; }

        [ObservableProperty] private ThemeVariant? _baseTheme;
        [ObservableProperty] private ViewModelBase _content;
        [ObservableProperty] private bool _areSettingsVisible;
        [ObservableProperty] private bool _canExit;
        [ObservableProperty] private bool _isLoggedIn;
        [ObservableProperty] private bool _isMenuVisible;
        [ObservableProperty] private bool _isTitleBarVisible;
        [ObservableProperty] private string _baseThemeText = "";
        [ObservableProperty] private string _title = "";

        private readonly Session _session;
        private readonly Settings _settings;
        private readonly SukiTheme _theme;
        private readonly RestartOnCrashService _restartOnCrashService;
        private readonly DataBaseSyncService _dataBaseSyncService;

        public MainWindowViewModel(
            SplashViewModel splashViewModel,
            ISukiDialogManager dialogManager,
            ISukiToastManager toastManager,
            Session session,
            Settings settings,
            RestartOnCrashService restartOnCrashService,
            DataBaseSyncService dataBaseSyncService)
        {
            this._settings = settings;
            this._session = session;
            this._theme = SukiTheme.GetInstance();
            this._theme.ChangeBaseTheme(ThemeVariant.Light);
            this._restartOnCrashService = restartOnCrashService;
            this._dataBaseSyncService = dataBaseSyncService;
            this.Content = splashViewModel;
            this.IsTitleBarVisible = false;
            this.ToastManager = toastManager;
            this.DialogManager = dialogManager;
            this.UpdateBaseTheme();
            this.UpdateTitle();
            this.IsActive = true;
        }

        protected override void SetupReactiveExtensions()
        {
            this._session
                .LoggedUser
                .Do(_ => this.UpdateTitle())
                .Do(user => this.AreSettingsVisible = user.HasPermission(PermissionType.OpenSettingsConfig))
                .Do(user => this.CanExit = user.HasPermission(PermissionType.Exit))
                .Do(user => this.IsLoggedIn = !user.IsNull)
                .Subscribe()
                .AddTo(ref Disposables);

            this._session
                .IsDatabaseOnline
                .Do(_ => this.UpdateTitle())
                .Subscribe()
                .AddTo(ref Disposables);

            if (_settings.AutoSyncDatabase)
            {
                Observable.Interval(TimeSpan.FromHours(1))
                    .SelectAwait(async (_, __) => await this.SyncDb())
                    .Subscribe()
                    .AddTo(ref Disposables);
            }
        }

        protected override void OnActivated()
        {
            Messenger.Register<SplashClosedMessage>(this, this.OnSplashClosed);
            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            Messenger.UnregisterAll(this);
            base.OnDeactivated();
        }

        private void OnSplashClosed(object recipient, SplashClosedMessage message)
        {
            this.IsMenuVisible = true;
            this.IsTitleBarVisible = true;
        }

        private void UpdateTitle()
        {
            var title = $"{Resources.txt_hermes} - {_settings.Station} - {_settings.Line}";
            if (!this._session.LoggedUser.Value.IsNull)
            {
                title += $"     (👤{this._session.LoggedUser.Value.Name})";
            }

            Title =
                $"{title} ({(this._session.IsDatabaseOnline.Value ? Resources.txt_online : Resources.txt_offline)})";
        }

        [RelayCommand]
        private void Logout()
        {
            this._session.Logout();
            this.OpenLogin();
        }

        [RelayCommand]
        private void OpenLogin()
        {
            Messenger.Send(new NavigateMessage(typeof(LoginViewModel)));
        }

        [RelayCommand]
        private void ToggleBaseTheme()
        {
            this._theme.SwitchBaseTheme();
            this.UpdateBaseTheme();
        }

        private void UpdateBaseTheme()
        {
            this.BaseTheme = _theme.ActiveBaseTheme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
            this.BaseThemeText = BaseTheme == ThemeVariant.Dark ? Resources.txt_dark_theme : Resources.txt_light_theme;
        }

        [RelayCommand]
        private void ToggleTitleBar()
        {
            IsTitleBarVisible = !IsTitleBarVisible;
            this.ShowInfoToast(
                IsTitleBarVisible
                    ? Resources.c_main_window_title_bar_vissible
                    : Resources.c_main_window_title_bar_hidden,
                IsTitleBarVisible
                    ? Resources.c_main_window_title_bar_visible_msg
                    : Resources.c_main_window_title_bar_hidden_msg
            );
        }

        [RelayCommand]
        private void Exit(SukiWindow window)
        {
            Messenger.Send(new ExitMessage());
            CanClose = true;
            window.Close();
        }

        [RelayCommand]
        private void ShowSettings()
        {
            Messenger.Send(new ShowSettingsMessage());
        }

        [RelayCommand]
        private async Task StartRestartOnCrash()
        {
            try
            {
                await _restartOnCrashService.Start();
                this.ShowSuccessToast(Resources.msg_restart_on_crash_started);
            }
            catch (Exception e)
            {
                this.ShowErrorToast(e.Message);
            }
        }

        private bool _isSyncing;

        [RelayCommand]
        private async Task<bool> SyncDb()
        {
            if (_isSyncing) return false;

            try
            {
                _isSyncing = true;
                var toast = ToastManager.CreateToast()
                    .WithTitle(Resources.txt_syncing_database)
                    .WithLoadingState(true)
                    .WithContent(Resources.txt_loading)
                    .Queue();
                await _dataBaseSyncService.Sync(progress =>
                    toast.Content = progress);
                await Task.Delay(TimeSpan.FromSeconds(2));
                ToastManager.Dismiss(toast);
                this.ShowSuccessToast(Resources.txt_sync_database);
                return true;
            }
            catch (Exception e)
            {
                this.ShowErrorToast(e.Message);
                return false;
            }
            finally
            {
                _isSyncing = false;
            }
        }
    }
}