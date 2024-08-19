﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;

namespace Hermes.TokenGen.Android;

[Activity(
    Label = "Hermes TokenGen",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}