using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Hermes.Utils.Parsers;

namespace Hermes.Models;

public class UnitUnderTest
{
    public static readonly UnitUnderTest Null = new NullUnitTest();

    [Key] public int Id { get; set; }
    [MaxLength(250)] public string FileName { get; init; }
    [MaxLength(250)] public string SerialNumber { get; init; } = string.Empty;
    public bool IsFail { get; init; }
    public List<Defect> Defects { get; init; } = [];
    [NotMapped] public string Content { get; init; }
    [NotMapped] public bool IsNull => this == Null;

    public UnitUnderTest(string fileName, string content, IUnitUnderTestParser parser) : this(fileName, content)
    {
        this.IsFail = parser.ParseIsFail(content);
        this.SerialNumber = parser.ParseSerialNumber(content);
        this.Defects = parser.ParseDefects(content);
    }

    public UnitUnderTest(string fileName, string content)
    {
        this.FileName = fileName;
        this.Content = content;
    }

    public UnitUnderTest() : this(string.Empty, string.Empty)
    {
    }
}

public class NullUnitTest() : UnitUnderTest(string.Empty, string.Empty);