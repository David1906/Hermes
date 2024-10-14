﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hermes.Cipher.Types;
using Hermes.Language;
using Hermes.Models;
using Hermes.Repositories;
using Hermes.Types;
using Material.Icons;
using System.Threading.Tasks;

namespace Hermes.Features.Login;

public partial class LoginViewModel : PageBase
{
    [ObservableProperty] private User _user = User.Null;
    [ObservableProperty] private string _userName = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private bool _isLoggedIn;
    [ObservableProperty] private bool _isLoggingIn;
    [ObservableProperty] private DepartmentType _department;
    private readonly Session _session;
    private readonly UserProxy _userProxy;

    public LoginViewModel(
        Session session,
        UserProxy userProxy) :
        base(
            Resources.txt_account,
            MaterialIconKind.Account,
            PermissionType.FreeAccess,
            0)
    {
        this._session = session;
        this._userProxy = userProxy;
        session.UserChanged += OnSessionUserChanged;
#if DEBUG
        LoginDebugUser();
#endif
    }

    [RelayCommand]
    private async Task Login()
    {
        await Task.Run(async () =>
        {
            try
            {
                IsLoggingIn = true;
                var user = await _userProxy.FindUser(this.UserName, this.Password);
                IsLoggedIn = !user.IsNull;
                _session.UpdateUser(user);
                if (!IsLoggedIn)
                {
                    this.ShowErrorToast(Resources.msg_invalid_user_password);
                }
                else
                {
                    this.UserName = string.Empty;
                    this.Password = string.Empty;
                }
            }
            finally
            {
                IsLoggingIn = false;
            }
        });
    }

    [RelayCommand]
    private void Logout()
    {
        this._session.Logout();
    }

    private void OnSessionUserChanged(User user)
    {
        this.User = user;
        this.IsLoggedIn = _session.IsLoggedIn;
    }

    partial void OnIsLoggedInChanged(bool value)
    {
        if (value)
        {
            LoginDebugUser();
        }
    }

    private void LoginDebugUser()
    {
        _session.UpdateUser(new DebugUser());
    }
}