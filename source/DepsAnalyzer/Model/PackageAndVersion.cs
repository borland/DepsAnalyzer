using System.Text.Json;
using System.Text.Json.Serialization;

namespace DepsAnalyzer.Model;

public record PackageAndVersion(string Package, string Version);
