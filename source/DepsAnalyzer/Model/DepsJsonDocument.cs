using System.Text.Json;
using System.Text.Json.Serialization;

namespace DepsAnalyzer.Model;

public class DepsJsonDocument
{
    // runtimeTarget property exists but isn't used

    // compilationOptions property exists but isn't used

    // Targets in the JSON looks like this:
    // "targets": {
    //     ".NETCoreApp,Version=v6.0": { // we parse this string into NetFrameworkMoniker
    //         "Octopus.Server/1.0.0": { // we parse this string into ExecutableAndVersion
    [JsonConverter(typeof(TargetsDictionaryConverter))]
    public Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>> Targets { get; set; } = null!;
}

public class TargetsDictionaryConverter : JsonConverter<Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>>
{
    public override Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        var result = new Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return result;

            var keyString = reader.GetString() ?? throw new JsonException("Expected to read string");
            var key = new NetFrameworkMoniker(keyString);

            reader.Read();

            var value = ReadInner(ref reader, typeToConvert, options) ?? throw new JsonException("Expected to read Dictionary<PackageVersion, Target>");

            result.Add(key, value);
        }

        return null;
    }
    
    static Dictionary<PackageAndVersion, Target>? ReadInner(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        var result = new Dictionary<PackageAndVersion, Target>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return result;

            var keyString = reader.GetString() ?? throw new JsonException("Expected to read string");
            var components = keyString.Split("/");
            if(components.Length != 2) throw new JsonException("Expected to read string separated by /, such as \"Octopus.Server/1.0.0\"");
            var key = new PackageAndVersion(components[0], components[1]);
            
            // bail back out to the system generator for the child "Target" as it doesn't have custom serialization behaviour
            reader.Read();
            
            var itemValue = JsonSerializer.Deserialize(ref reader, DepsJsonSerializerContext.Context.Target) ?? throw new JsonException("Expected to read Target");

            result.Add(key, itemValue);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<NetFrameworkMoniker, Dictionary<PackageAndVersion, Target>> value, JsonSerializerOptions options) => throw new NotImplementedException();
}