using Hermes.Common.Extensions;
using Hermes.Types;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;

namespace Hermes.Models;

public class SfcResponse
{
    private readonly string _additionalOkResponse = "";
    public static readonly SfcResponse Null = new SfcResponseNull();

    private const RegexOptions RgxOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline;
    private static readonly Regex RegexWrongStation = new(@"^go-.+[\r\n]+", RgxOptions);
    private static readonly Regex RegexIsOk = new(@"^ok[\r\n]+", RgxOptions);
    private static readonly Regex RegexIsEndOfFileError = new(@"end-of-file", RgxOptions);
    public const string TimeoutText = "Timeout";
    public const string ScanError = "ScanError";

    [Key] public int Id { get; init; }

    public bool IsOk => !IsFail ||
                        (!string.IsNullOrEmpty(_additionalOkResponse) && Content.Contains(_additionalOkResponse));

    public virtual bool IsFail => this.Type != SfcResponseType.Ok;
    public SfcResponseType Type { get; init; }
    [MaxLength(3000)] public string Content { get; init; } = "";
    [MaxLength(512)] public string FullPath { get; set; } = "";
    [NotMapped] public string FileName => Path.GetFileName(this.FullPath);

    [NotMapped]
    public string Details =>
        IsFail ? $"{Type.ToTranslatedString()} - {Type.GetTranslatedDescription()}" : "";

    [NotMapped] public bool IsNull => this == Null;
    [NotMapped] public bool IsTimeout => Type == SfcResponseType.Timeout;
    public bool IsEndOfFileError => RegexIsEndOfFileError.IsMatch(Content);

    public SfcResponse()
    {
    }

    public SfcResponse(string content, string fullPath = "", string additionalOkResponse = "")
    {
        this.FullPath = fullPath;
        this.Content = content;
        this._additionalOkResponse = additionalOkResponse;
        this.Type = ParseErrorType(content);
    }

    private static SfcResponseType ParseErrorType(string content)
    {
        if (RegexIsOk.Match(content).Success)
        {
            return SfcResponseType.Ok;
        }

        if (RegexWrongStation.Match(content).Success)
        {
            return SfcResponseType.WrongStation;
        }

        if (content == TimeoutText)
        {
            return SfcResponseType.Timeout;
        }

        if (content == ScanError)
        {
            return SfcResponseType.ScanError;
        }

        return SfcResponseType.Unknown;
    }

    public static SfcResponse BuildTimeout()
    {
        return new SfcResponse(TimeoutText);
    }
}

public class SfcResponseNull() : SfcResponse();