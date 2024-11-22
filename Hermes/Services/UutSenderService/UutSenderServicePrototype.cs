using Hermes.Models;
using Hermes.Types;

namespace Hermes.Services.UutSenderService;

public class UutSenderServicePrototype(
    DefaultUutSenderService defaultUutSenderService,
    GkgUutSenderService gkgUutSenderService,
    LabelingMachineUutSenderService labelingMachineUutSenderService,
    Settings settings)
{
    public UutSenderService Build()
    {
        switch (settings.Machine)
        {
            case MachineType.Labeling:
                return labelingMachineUutSenderService;
            case MachineType.ScreenPrinter:
                return gkgUutSenderService;
            default:
                return defaultUutSenderService;
        }
    }
}