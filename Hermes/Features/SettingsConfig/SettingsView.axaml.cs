using System;
using Avalonia.Controls;
using ConfigFactory.Models;
using ConfigFactory;

namespace Hermes.Features.SettingsConfig;

public partial class SettingsView : Window
{
    public bool CanClose { get; set; }
    private bool _isClosed;

    public SettingsView()
    {
        InitializeComponent();
        this.Closing += Window_OnClosing;
        this.Closed += (_, _) => this._isClosed = true;
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
            this.Hide();
        }
    }

    public void Append(SettingsConfigModel settingsConfigModel)
    {
        if (ConfigPage.DataContext is ConfigPageModel model)
        {
            model.Append(settingsConfigModel);
        }
    }

    public void ForceClose()
    {
        try
        {
            if (this._isClosed) return;
            this.CanClose = true;
            this.Close();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}