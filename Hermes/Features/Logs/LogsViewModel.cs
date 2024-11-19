using Hermes.Language;
using Material.Icons;

namespace Hermes.Features.Logs;

public partial class LogsViewModel(UnitUnderTestLogViewModel underTestLogViewModel, StopLogViewModel stopLogViewModel)
    : PageBase(
        Resources.txt_uut_processor,
        MaterialIconKind.History)
{
    public UnitUnderTestLogViewModel UnitUnderTestLogViewModel { get; set; } = underTestLogViewModel;
    public StopLogViewModel StopLogViewModel { get; set; } = stopLogViewModel;
}