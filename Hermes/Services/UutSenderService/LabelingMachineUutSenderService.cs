using System.Threading.Tasks;
using Hermes.Builders;
using Hermes.Common;
using Hermes.Models;

namespace Hermes.Services.UutSenderService;

public class LabelingMachineUutSenderService : DefaultUutSenderService
{
    private readonly FileService _fileService;

    public LabelingMachineUutSenderService(
        FolderWatcherService folderWatcherService,
        ILogger logger,
        ISfcService sfcService,
        Settings settings,
        SfcResponseBuilder sfcResponseBuilder,
        UnitUnderTestBuilder unitUnderTestBuilder,
        FileService fileService)
        : base(folderWatcherService, logger, sfcService, settings, sfcResponseBuilder, unitUnderTestBuilder)
    {
        this._fileService = fileService;
    }

    protected override async Task SendUnitUnderTest(TextDocument textDocument)
    {
        await base.SendUnitUnderTest(textDocument);
        var sfcResponse = this.UnitUnderTest.Value.SfcResponse;
        if (sfcResponse is { IsNull: false })
        {
            await this._fileService.WriteResponseToInputPathAsync(
                this.UnitUnderTest.Value.FileName,
                sfcResponse.Content);
        }
    }
}