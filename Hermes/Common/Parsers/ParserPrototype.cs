using Hermes.Types;
using System.Collections.Generic;

namespace Hermes.Common.Parsers;

public class ParserPrototype
{
    private readonly Dictionary<LogfileType, IUnitUnderTestParser> _parsersDictionary = new();

    public ParserPrototype(
        TriUnitUnderTestParser triUnitUnderTestParser,
        LabelingMachineUnitUnderTestParser labelingMachineUnitUnderTestParser,
        GkgUnitUnderTestParser gkgUnitUnderTestParser)
    {
        this._parsersDictionary.Add(LogfileType.TriDefault,
            triUnitUnderTestParser);
        this._parsersDictionary.Add(LogfileType.LabelingMachineDefault,
            labelingMachineUnitUnderTestParser);
        this._parsersDictionary.Add(LogfileType.GkgDefault,
            gkgUnitUnderTestParser);
    }

    public IUnitUnderTestParser? GetUnitUnderTestParser(LogfileType logfileType)
    {
        return _parsersDictionary.TryGetValue(logfileType, out var parser) ? parser : null;
    }
}