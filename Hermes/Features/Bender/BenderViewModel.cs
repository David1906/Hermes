using Material.Icons;
using R3;

namespace Hermes.Features.Bender;

public class BenderViewModel : PageBase
{
    public PackageTrackingViewModel PackageTrackingViewModel { get; set; }
    public PackageScannerViewModel PackageScannerViewModel { get; set; }

    public BenderViewModel(PackageTrackingViewModel packageTrackingViewModel,
        PackageScannerViewModel packageScannerViewModel) : base(
        "Bender",
        MaterialIconKind.Qrcode,
        3)
    {
        this.PackageTrackingViewModel = packageTrackingViewModel;
        this.PackageScannerViewModel = packageScannerViewModel;
        this.IsActive = true;
    }

    protected override void SetupReactiveExtensions()
    {
        this.PackageScannerViewModel
            .ObservePropertyChanged(x => x.PackageScanned)
            .Skip(1)
            .SubscribeAwait(async (_, _) => await this.PackageTrackingViewModel.DataReload())
            .AddTo(ref Disposables);
    }
}