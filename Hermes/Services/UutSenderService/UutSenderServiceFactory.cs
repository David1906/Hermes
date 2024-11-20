using Hermes.Models;
using Hermes.Types;

namespace Hermes.Services.UutSenderService;

public class UutSenderServiceFactory(
    DefaultUutSenderService defaultUutSenderService,
    GkgUutSenderService gkgUutSenderService,
    Settings settings)
{
    public UutSenderService Build()
    {
        return settings.Machine is MachineType.ScreenPrinter
            ? gkgUutSenderService
            : defaultUutSenderService;
    }
}