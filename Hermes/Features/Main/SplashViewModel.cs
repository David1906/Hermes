using Hermes.Common.Messages;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hermes.Language;

namespace Hermes.Features.Main;

public partial class SplashViewModel : ViewModelBase
{
    [ObservableProperty] private string _text = Resources.txt_loading;

    public SplashViewModel()
    {
        this.IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<SplashMessage>(this, this.OnSplashMessage);
        base.OnActivated();
    }

    protected override void OnDeactivated()
    {
        Messenger.UnregisterAll(this);
        base.OnDeactivated();
    }

    private void OnSplashMessage(object recipient, SplashMessage message)
    {
        this.Text = string.IsNullOrEmpty(message.Value) ? Resources.txt_loading : message.Value;
    }
}