using Avalonia.Controls;
using Avalonia.Input;
using SukiUI.Controls;

namespace Hermes.Features;

public partial class MainWindowView : SukiWindow
{
    public MainWindowView()
    {
        InitializeComponent();
        IsTitleBarVisible = false;
        IsMenuVisible = false;
    }

    private void MakeFullScreenPressed(object? sender, PointerPressedEventArgs e)
    {
        WindowState = WindowState == WindowState.FullScreen ? WindowState.Normal : WindowState.FullScreen;
        IsTitleBarVisible = WindowState != WindowState.FullScreen;
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (this.CanClose)
        {
            e.Cancel = false;
        }
        else
        {
            e.Cancel = true;
            (this.DataContext as MainWindowViewModel)?.LogoutCommand.Execute(null);
            this.Hide();
        }
    }

    private bool CanClose => (this.DataContext as MainWindowViewModel)?.CanClose ?? true;
}