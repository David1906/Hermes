using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hermes.Common.Extensions;
using Hermes.Common.Messages;
using Hermes.Common.Parsers;
using Hermes.Common;
using Hermes.Language;
using Hermes.Models;
using Hermes.Repositories;
using SukiUI.Dialogs;
using System.Threading.Tasks;
using System;

namespace Hermes.Features.Bender;

public partial class PackageScannerViewModel : ViewModelBase
{
    public event Action<Package>? PackageScanned;
    public event Action<string>? InstructionsChanged;

    [ObservableProperty] private Bitmap? _cover;
    [ObservableProperty] private Package _package = Package.Null;
    [ObservableProperty] private string _scannedCode = "";
    [ObservableProperty] private bool _isCodeGenerated;
    [ObservableProperty] private string _instructions = Resources.msg_change_wo;
    [ObservableProperty] private WorkOrder _workOrder = WorkOrder.Null;
    [ObservableProperty] private Bitmap? _partNumberImage;
    [ObservableProperty] private Bitmap? _revisionImage;
    [ObservableProperty] private Bitmap? _workOrderImage;

    private readonly PackageParser _packageParser;
    private readonly QrGenerator _qrGenerator;
    private readonly ISukiDialogManager _dialogManager;
    private readonly ISfcRepository _sfcRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ILogger _logger;

    public PackageScannerViewModel(
        PackageParser packageParser,
        QrGenerator qrGenerator,
        ISukiDialogManager dialogManager,
        ISfcRepository sfcRepository,
        ISettingsRepository settingsRepository,
        ILogger logger)
    {
        this._packageParser = packageParser;
        this._qrGenerator = qrGenerator;
        this._dialogManager = dialogManager;
        this._sfcRepository = sfcRepository;
        this._settingsRepository = settingsRepository;
        this._logger = logger;
    }


    [RelayCommand]
    private async Task ParsePackage()
    {
        this.Package = this._packageParser.Parse(this.ScannedCode);
        this.ScannedCode = "";
        await this.GenerateCode();
    }

    [RelayCommand]
    private async Task GenerateCode()
    {
        if (!this.Package.IsValid)
        {
            this.Cover = null;
            this.IsCodeGenerated = false;
            this.Instructions = Resources.msg_scan_vendor;
            return;
        }

        this.Cover = await this._qrGenerator.GenerateAvaloniaBitmap(
            this.Package.ToString(), 150);
        this.IsCodeGenerated = true;
        await this.AddPackageToSfc();
        this.Instructions = Resources.msg_scan_2d_package;
    }

    private async Task AddPackageToSfc()
    {
        try
        {
            var package = await _sfcRepository.FindPackageTracking(this.Package.NormalizedId);
            if (package.IsNull)
            {
                Package.Line = _settingsRepository.Settings.Line.ToUpperString();
                await _sfcRepository.AddPackageTrack(Package);
            }
            else
            {
                await _sfcRepository.UpdatePackageTrackingLine(
                    package.NormalizedId,
                    _settingsRepository.Settings.Line.ToUpperString());
            }

            Messenger.Send(new ShowToastMessage("Success", "Package added to Hermes", NotificationType.Success));
            PackageScanned?.Invoke(Package);
        }
        catch (Exception e)
        {
            Messenger.Send(new ShowToastMessage("Error", e.Message, NotificationType.Error));
            _logger.Error(e.Message);
        }
    }

    [RelayCommand]
    private void ChangeWorkOrder()
    {
        _dialogManager.CreateDialog()
            .WithViewModel(dialog =>
            {
                var vm = new WorkOrderDialogViewModel(dialog, _sfcRepository);
                vm.WorkOrderSelected += (workOrder) => Task.Run(() => this.SetWorkOrder(workOrder));
                return vm;
            })
            .TryShow();
    }

    private async Task SetWorkOrder(WorkOrder workOrder)
    {
        this.WorkOrder = workOrder;
        if (this.WorkOrder.IsNull)
        {
            return;
        }

        this.PartNumberImage = await this._qrGenerator.GenerateAvaloniaBitmap((this.WorkOrder.PartNumber), 50);
        this.RevisionImage = await this._qrGenerator.GenerateAvaloniaBitmap((this.WorkOrder.Revision), 50);
        this.WorkOrderImage = await this._qrGenerator.GenerateAvaloniaBitmap((this.WorkOrder.Id), 50);
        this.Instructions = Resources.msg_scan_2d_package;
    }

    partial void OnInstructionsChanged(string value)
    {
        Dispatcher.UIThread.Invoke(() => { this.InstructionsChanged?.Invoke(value); });
    }
}