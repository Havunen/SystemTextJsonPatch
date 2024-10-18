using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SystemTextJsonPatch;

public class NameCasingTestObject
{
	public int PropertyName { get; set; }

	public int propertyName { get; set; }

}

public class AttrNameCasingTestObject
{
	[JsonPropertyName("propertyName")]
	public int PropertyName { get; set; }

	public int propertyName { get; set; }

}