using System;
using System.Text.Json;

namespace SystemTextJsonPatch;

/// <summary>
/// Metadata for JsonProperty.
/// </summary>
public class JsonPatchProperty
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public JsonPatchProperty(JsonProperty property, object parent)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        Property = property;
        Parent = parent;
    }

    /// <summary>
    /// Gets or sets JsonProperty.
    /// </summary>
    public JsonProperty Property { get; set; }

    /// <summary>
    /// Gets or sets Parent.
    /// </summary>
    public object Parent { get; set; }
}
