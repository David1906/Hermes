using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hermes.Common.Extensions;
using Hermes.Common.Parsers;
using Hermes.Common;
using Hermes.Language;
using Hermes.Models;
using Hermes.Repositories;
using R3;
using SukiUI.Dialogs;
using System.Threading.Tasks;
using System;

namespace Hermes.Features.Bender;

public partial class PackageScannerViewModel : ViewModelBase
{
    [ObservableProperty] private Bitmap? _cover;
    [ObservableProperty] private Bitmap? _partNumberImage;
    [ObservableProperty] private Bitmap? _revisionImage;
    [ObservableProperty] private Bitmap? _workOrderImage;
    [ObservableProperty] private Package _package = Package.Null;
    [ObservableProperty] private WorkOrder _workOrder = WorkOrder.Null;
    [ObservableProperty] private bool _isCodeGenerated;
    [ObservableProperty] private string _scannedCode = "";
    [ObservableProperty] private string _instructions = Resources.msg_change_wo;
    [ObservableProperty] private string _packageScanned = Resources.msg_change_wo;

    private readonly ILogger _logger;
    private readonly ISfcRepository _sfcRepository;
    private readonly ISukiDialogManager _dialogManager;
    private readonly PackageParser _packageParser;
    private readonly QrGenerator _qrGenerator;
    private readonly Settings _settings;

    public PackageScannerViewModel(
        ILogger logger,
        ISfcRepository sfcRepository,
        ISukiDialogManager dialogManager,
        PackageParser packageParser,
        QrGenerator qrGenerator,
        Settings settings)
    {
        this._dialogManager = dialogManager;
        this._logger = logger;
        this._packageParser = packageParser;
        this._qrGenerator = qrGenerator;
        this._settings = settings;
        this._sfcRepository = sfcRepository;
    }

    [RelayCommand]
    private async Task ParsePackage()
    {
        this.Package = this._packageParser.Parse(this.ScannedCode.ToUpper());
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
                Package.Line = _settings.Line.ToUpperString();
                await _sfcRepository.AddPackageTrack(Package);
            }
            else
            {
                await _sfcRepository.UpdatePackageTrackingLine(
                    package.NormalizedId,
                    _settings.Line.ToUpperString());
            }

            this.ShowSuccessToast(Resources.msg_package_added_to_hermes);
            this.PackageScanned = this.Package.NormalizedId;
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
            this.ShowErrorToast(e.Message);
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
}