using System.Text.Json.Serialization;

namespace Domain.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SiteSource
{
    Sefon,
    Hitmo,
    C
}