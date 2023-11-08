using System.Text.Json;
using System.Text.Json.Serialization;

namespace DepsAnalyzer.Model;

[JsonSerializable(typeof(DepsJsonDocument))]
[JsonSerializable(typeof(NetFrameworkMoniker))]
[JsonSerializable(typeof(PackageAndVersion))]
[JsonSerializable(typeof(Target))]
public partial class DepsJsonSerializerContext : JsonSerializerContext
{
    // This goes away in .NET 8 as the JsonSourceGenerationOptionsAttribute has all the things we'd like
    // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/Common/JsonSourceGenerationOptionsAttribute.cs
    public static DepsJsonSerializerContext Context { get; } = new (new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
    });
}