using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hermes.Common.Messages;
using Hermes.Models;
using R3;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hermes.Common;

namespace Hermes.Features.Main;

public partial class MainViewModel : ViewModelBase
{
    public RangeObservableCollection<PageBase> ShownPages { get; set; } = [];
    [ObservableProperty] private PageBase? _activePage;

    private readonly PagePrototype _pagePrototype;
    private readonly Session _session;

    public MainViewModel(
        PagePrototype pagePrototype,
        Session session)
    {
        this._pagePrototype = pagePrototype;
        this._session = session;
        this.IsActive = true;
    }

    protected override void SetupReactiveExtensions()
    {
        this._session
            .LoggedUser
            .Do(this.ConfigurePages)
            .Subscribe()
            .AddTo(ref Disposables);
    }

    protected override void OnActivated()
    {
        Messenger.Register<NavigateMessage>(this, this.OnNavigateMessage);
        base.OnActivated();
    }

    private void OnNavigateMessage(object recipient, NavigateMessage message)
    {
        var viewModel = this.ShownPages
            .FirstOrDefault(x => message.Value == x.GetType());
        if (viewModel is null) return;
        this.ActivePage = viewModel;
    }

    private void ConfigurePages(User user)
    {
        var visiblePages = this._pagePrototype.GetPages(user);
        this.UpdatePages(visiblePages);
    }

    private void UpdatePages(List<PageBase> visiblePages)
    {
        this.ClearPages(visiblePages);
        this.ShownPages.AddRange(visiblePages
            .Except(this.ShownPages)
            .OrderBy(x => x.Index)
            .ToList());
        visiblePages.ForEach(x => { x.IsActive = true; });
    }

    private void ClearPages(List<PageBase> visiblePages)
    {
        var pagesToRemove = this.ShownPages
            .Except(visiblePages)
            .ToList();
        pagesToRemove.ForEach(x => { x.IsActive = false; });
        this.ShownPages.RemoveRange(pagesToRemove);
    }
}