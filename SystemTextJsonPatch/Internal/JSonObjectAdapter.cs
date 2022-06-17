using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SystemTextJsonPatch.Internal;

public class JSonObjectAdapter : IAdapter
{
    public bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions options,
        object value,
        out string errorMessage)
    {
        var obj = (JsonObject)target;

        obj[segment] = value != null ? JsonNode.Parse(JsonSerializer.Serialize(value, options)) : null;

        errorMessage = null;
        return true;
    }

    public bool TryGet(
        object target,
        string segment,
        JsonSerializerOptions options,
        out object value,
        out string errorMessage)
    {
        var obj = (JsonObject)target;

        if (!obj.TryGetPropertyValue(segment, out var valueAsToken))
        {
            value = null;
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        value = valueAsToken;
        errorMessage = null;
        return true;
    }

    public bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions options,
        out string errorMessage)
    {
        var obj = (JsonObject)target;

        if (!obj.Remove(segment))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        errorMessage = null;
        return true;
    }

    public bool TryReplace(
        object target,
        string segment,
        JsonSerializerOptions options,
        object value,
        out string errorMessage)
    {
        var obj = (JsonObject)target;

        if (obj[segment] == null)
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }
        
        obj[segment] = value != null ? JsonNode.Parse(JsonSerializer.Serialize(value, options)) : null;

        errorMessage = null;
        return true;
    }

    public bool TryTest(
        object target,
        string segment,
        JsonSerializerOptions options,
        object value,
        out string errorMessage)
    {
        var obj = (JsonObject)target;

        if (!obj.TryGetPropertyValue(segment, out var currentValue))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (currentValue == null || string.IsNullOrEmpty(currentValue.ToString()))
        {
            errorMessage = Resources.FormatValueForTargetSegmentCannotBeNullOrEmpty(segment);
            return false;
        }
        
        if (!Equals(currentValue, value) && !string.Equals(JsonSerializer.SerializeToNode(currentValue)?.ToString(), JsonSerializer.SerializeToNode(value)?.ToString(), StringComparison.Ordinal))
        {
            errorMessage = Resources.FormatValueNotEqualToTestValue(JsonSerializer.SerializeToNode(currentValue)?.ToString(), value, segment);
            return false;
        }

        errorMessage = null;
        return true;
    }

    public bool TryTraverse(
        object target,
        string segment,
        JsonSerializerOptions options,
        out object nextTarget,
        out string errorMessage)
    {
        var obj = (JsonObject)target;

        if (!obj.TryGetPropertyValue(segment, out var nextTargetToken))
        {
            nextTarget = null;
            errorMessage = null;
            return false;
        }

        nextTarget = nextTargetToken;
        errorMessage = null;
        return true;
    }
}
