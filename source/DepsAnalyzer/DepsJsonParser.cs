using System.Text.Json;
using DepsAnalyzer.Model;

namespace DepsAnalyzer;

public class DepsJsonParser
{
    public static DepsJsonDocument? Parse(ReadOnlySpan<byte> utf8FileContents)
    {
        return JsonSerializer.Deserialize(utf8FileContents, Model.DepsJsonSerializerContext.Context.DepsJsonDocument);
    }
}