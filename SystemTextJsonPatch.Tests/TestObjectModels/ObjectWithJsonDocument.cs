using System.Text.Json;

namespace SystemTextJsonPatch;

public class ObjectWithJsonDocument
{
    public JsonDocument CustomData { get; set; } = JsonDocument.Parse("{}");
}
