using Avalonia.Controls;
using SukiUI.Controls;

namespace Hermes.Features.UutProcessor;

public partial class StopView : SukiWindow
{
    public bool CanClose { get; set; }

    public StopView()
    {
        InitializeComponent();
        Closing += (_, args) => args.Cancel = !CanClose;
#if DEBUG
        this.Topmost = false;
        this.CanClose = true;
#endif
    }

    public void ForceClose()
    {
        this.CanClose = true;
        this.Close();
    }
}