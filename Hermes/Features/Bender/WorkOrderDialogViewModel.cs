using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hermes.Models;
using Hermes.Repositories;
using SukiUI.Dialogs;
using System.Threading.Tasks;
using System;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using Hermes.Common.Messages;

namespace Hermes.Features.Bender;

public partial class WorkOrderDialogViewModel : ViewModelBase
{
    public event Action<WorkOrder>? WorkOrderSelected;
    [ObservableProperty] private bool _isWorkOrderFound = true;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(FindWorkOrderCommand))]
    private string _workOrderText = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AcceptCommand))]
    private WorkOrder _workOrder = WorkOrder.Null;

    [ObservableProperty] private bool _isWorkOrderLoading;

    private readonly ISukiDialog _dialog;
    private readonly ISfcRepository _sfcRepository;

    public WorkOrderDialogViewModel(ISukiDialog dialog, ISfcRepository sfcRepository)
    {
        this._dialog = dialog;
        this._sfcRepository = sfcRepository;
    }

    [RelayCommand(CanExecute = nameof(CanFindWorkOrder))]
    private async Task FindWorkOrder()
    {
        try
        {
            IsWorkOrderLoading = true;
            var normalizedWorkOrder = Package.NormalizeWorkOrder(this.WorkOrderText);
            this.WorkOrder = await _sfcRepository.FindWorkOrder(normalizedWorkOrder);
        }
        catch (Exception)
        {
            Messenger.Send(new ShowToastMessage("Error", "An error occurred while getting the work order information",
                NotificationType.Error));
        }
        finally
        {
            IsWorkOrderLoading = false;
            IsWorkOrderFound = !this.WorkOrder.IsNull;
        }
    }

    private bool CanFindWorkOrder => !string.IsNullOrWhiteSpace(WorkOrderText);

    [RelayCommand]
    private void CloseDialog() => _dialog.Dismiss();

    [RelayCommand(CanExecute = nameof(CanAccept))]
    private void Accept()
    {
        WorkOrderSelected?.Invoke(WorkOrder);
        _dialog.Dismiss();
    }

    private bool CanAccept => !WorkOrder.IsNull;
}