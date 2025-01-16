using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SystemTextJsonPatch.Internal;

public sealed class JSonObjectAdapter : IAdapter
{
	public bool TryAdd(object target, string segment, JsonSerializerOptions options, object? value, out string? errorMessage)
	{
		var obj = (JsonObject)target;

		var propertyName = FindPropertyName(obj, segment, options);

		obj[propertyName] = value != null ? JsonSerializer.SerializeToNode(value, options) : null;

		errorMessage = null;
		return true;
	}

	public bool TryGet(object target, string segment, JsonSerializerOptions options, out object? value, out string? errorMessage)
	{
		var obj = (JsonObject)target;

		var propertyName = FindPropertyName(obj, segment, options);

		if (!obj.TryGetPropertyValue(propertyName, out var valueAsToken))
		{
			value = null;
			errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
			return false;
		}

		value = valueAsToken;
		errorMessage = null;
		return true;
	}

	public bool TryRemove(object target, string segment, JsonSerializerOptions options, out string? errorMessage)
	{
		var obj = (JsonObject)target;

		var propertyName = FindPropertyName(obj, segment, options);

		if (!obj.Remove(propertyName))
		{
			errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
			return false;
		}

		errorMessage = null;
		return true;
	}

	public bool TryReplace(object target, string segment, JsonSerializerOptions options, object? value, out string? errorMessage)
	{
		var obj = (JsonObject)target;

		var propertyName = FindPropertyName(obj, segment, options);

		if (obj[propertyName] == null)
		{
			errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
			return false;
		}

		obj[propertyName] = value != null ? JsonSerializer.SerializeToNode(value, options) : null;

		errorMessage = null;
		return true;
	}

	public bool TryTest(object target, string segment, JsonSerializerOptions options, object? value, out string? errorMessage)
	{
		var obj = (JsonObject)target;

		var propertyName = FindPropertyName(obj, segment, options);

		if (!obj.TryGetPropertyValue(propertyName, out var currentValue))
		{
			errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
			return false;
		}

		// all numeric values are handled as decimals
		if (ConversionResultProvider.IsNumericType(currentValue))
		{
			if (!Equals(currentValue, value))
			{
				errorMessage = Resources.FormatValueNotEqualToTestValue(JsonSerializer.SerializeToNode(currentValue)?.ToString(), value, segment);
				return false;
			}
		}
		else if (!ByteHelper.BytesEquals(JsonSerializer.SerializeToUtf8Bytes(currentValue), JsonSerializer.SerializeToUtf8Bytes(value)))
		{
			errorMessage = Resources.FormatValueNotEqualToTestValue(JsonSerializer.SerializeToNode(currentValue)?.ToString(), value, segment);
			return false;
		}

		errorMessage = null;
		return true;
	}

	public bool TryTraverse(object target, string segment, JsonSerializerOptions options, out object? nextTarget, out string? errorMessage)
	{
		var obj = (JsonObject)target;

		var propertyName = FindPropertyName(obj, segment, options);

		if (!obj.TryGetPropertyValue(propertyName, out JsonNode? nextTargetToken))
		{
			nextTarget = null;
			errorMessage = null;
			return false;
		}

		nextTarget = nextTargetToken;
		errorMessage = null;
		return true;
	}

	private static string FindPropertyName(JsonObject? obj, string segment, JsonSerializerOptions options)
	{
		if (!options.PropertyNameCaseInsensitive || obj == null)
			return segment;

		if (obj.ContainsKey(segment))
			return segment;

		foreach (var node in obj)
		{
			if (string.Equals(node.Key, segment, StringComparison.OrdinalIgnoreCase))
				return node.Key;
		}

		return segment;
	}
}
