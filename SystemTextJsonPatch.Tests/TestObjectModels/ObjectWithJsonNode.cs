using System.Text.Json.Nodes;

namespace SystemTextJsonPatch;

public class ObjectWithJsonNode
{
	public JsonNode CustomData { get; set; } = JsonNode.Parse("{}");
}
