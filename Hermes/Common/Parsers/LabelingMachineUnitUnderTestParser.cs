using Hermes.Models;
using Hermes.Repositories;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using Hermes.Common.Extensions;

namespace Hermes.Common.Parsers;

public class LabelingMachineUnitUnderTestParser : IUnitUnderTestParser
{
    public const string NoPackageAvailableText = "SNoPackageAvailable";
    private static readonly Regex PkgIdRgx = new(@"^(\s*)(S[\w,]*)([\r\n]*)");
    private static readonly Regex SerialNumberRgx = new(@"^\s*([\w]+)$[\r\n]*");

    private readonly ISfcRepository _sfcRepository;
    private readonly Settings _settings;
    private readonly ILogger _logger;

    public LabelingMachineUnitUnderTestParser(
        ISfcRepository sfcRepository,
        Settings settings,
        ILogger logger)
    {
        this._sfcRepository = sfcRepository;
        this._settings = settings;
        this._logger = logger;
    }

    public List<Defect> ParseDefects(string content)
    {
        return [];
    }

    public bool ParseIsFail(string content)
    {
        return false;
    }

    public string ParseSerialNumber(string content)
    {
        if (string.IsNullOrEmpty(content)) return string.Empty;
        var lines = content.Split(["\r\n"], StringSplitOptions.None);
        if (lines.Length < 2) return string.Empty;
        var match = SerialNumberRgx.Match(lines[2]);
        if (lines.Length < 3 || !match.Success) return string.Empty;
        return match.Groups[1].Value;
    }

    public string GetTestContent(string serialNumber, bool isPass, List<Defect>? defects = null)
    {
        return $"""
                SPKF3202406000000
                1
                {serialNumber}
                N/A
                N/A
                N/A
                N/A
                N/A
                N/A
                """;
    }

    public async Task<string> GetContentAsync(string content)
    {
        if (string.IsNullOrEmpty(content)) return content;

        var packageId = await GetPackageIdAsync(content);
        return PkgIdRgx.Replace(content, m => m.Groups[1].Value + packageId + m.Groups[3].Value);
    }

    public async Task<string> GetPackageIdAsync(string content)
    {
        var packageId = NoPackageAvailableText;
        try
        {
            var package = await this._sfcRepository
                .FindNextCanUsePackage(
                    _settings.Line.ToUpperString());
            if (!package.IsNull)
            {
                packageId = package.Id;
            }
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
        }

        return packageId;
    }
}