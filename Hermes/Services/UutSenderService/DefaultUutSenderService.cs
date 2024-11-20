using Hermes.Builders;
using Hermes.Common.Extensions;
using Hermes.Common;
using Hermes.Models;
using R3;
using System.Threading.Tasks;

namespace Hermes.Services.UutSenderService;

public class DefaultUutSenderService : UutSenderService
{
    private readonly FolderWatcherService _folderWatcherService;
    private readonly ILogger _logger;
    private readonly Settings _settings;
    private readonly UnitUnderTestBuilder _unitUnderTestBuilder;

    public DefaultUutSenderService(
        FolderWatcherService folderWatcherService,
        ILogger logger,
        ISfcService sfcService,
        Settings settings,
        SfcResponseBuilder sfcResponseBuilder,
        UnitUnderTestBuilder unitUnderTestBuilder)
        : base(logger, sfcService, settings, sfcResponseBuilder)
    {
        this._settings = settings;
        this._logger = logger;
        this._folderWatcherService = folderWatcherService;
        this._unitUnderTestBuilder = unitUnderTestBuilder;
    }

    public override string Path => _settings.InputPath;

    protected override void StartService()
    {
        this._folderWatcherService.Start(
            _settings.InputPath,
            "*" + this._settings.InputFileExtension.GetDescription());
    }

    protected override void StopService()
    {
        this._folderWatcherService.Stop();
    }

    public override async Task ReSend(UnitUnderTest unitUnderTest)
    {
        await this._folderWatcherService.ResendAsync(unitUnderTest);
    }

    public override bool CanReSend(UnitUnderTest unitUnderTest)
    {
        return this._folderWatcherService.FileExists(unitUnderTest.FullPath);
    }

    protected override void SetupReactiveExtensions()
    {
        this._folderWatcherService
            .TextDocumentCreated
            .Select(this.SendUnitUnderTest)
            .Subscribe()
            .AddTo(ref Disposables);
    }

    private async Task SendUnitUnderTest(TextDocument textDocument)
    {
        var unitUnderTest = await this._unitUnderTestBuilder.BuildAsync(textDocument);
        if (unitUnderTest.IsNull)
        {
            this._logger.Error($"Invalid file: {textDocument.FileName}");
            return;
        }

        unitUnderTest.SfcResponse = await SendUnitUnderTest(unitUnderTest);
        this.UnitUnderTest.Value = unitUnderTest;
    }
}