using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Internal.Proxies;

namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class PocoAdapter : IAdapter
{
    public bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions options,
        object value,
        out string errorMessage)
    {
        if (!TryGetJsonProperty(target, segment, options, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.CanWrite)
        {
            errorMessage = Resources.FormatCannotUpdateProperty(segment);
            return false;
        }

        if (!ConversionResultProvider.TryConvertTo(value, jsonProperty.PropertyType, options, out var convertedValue, out var conversionErrorMessage))
        {
            errorMessage = conversionErrorMessage ?? Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        jsonProperty.SetValue(target, convertedValue);

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
        if (!TryGetJsonProperty(target, segment, options, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            value = null;
            return false;
        }

        if (!jsonProperty.CanRead)
        {
            errorMessage = Resources.FormatCannotReadProperty(segment);
            value = null;
            return false;
        }

        value = jsonProperty.GetValue(target);
        errorMessage = null;
        return true;
    }

    public bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions options,
        out string errorMessage)
    {
        if (!TryGetJsonProperty(target, segment, options, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.CanWrite)
        {
            errorMessage = Resources.FormatCannotUpdateProperty(segment);
            return false;
        }

        // Setting the value to "null" will use the default value in case of value types, and
        // null in case of reference types
        object value = null;
        if (jsonProperty.PropertyType.IsValueType
            && Nullable.GetUnderlyingType(jsonProperty.PropertyType) == null)
        {
            value = Activator.CreateInstance(jsonProperty.PropertyType);
        }

        jsonProperty.SetValue(target, value);

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
        if (!TryGetJsonProperty(target, segment, options, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.CanWrite)
        {
            errorMessage = Resources.FormatCannotUpdateProperty(segment);
            return false;
        }

        if (!ConversionResultProvider.TryConvertTo(value, jsonProperty.PropertyType, options, out var convertedValue, out var conversionErrorMessage))
        {
            errorMessage = conversionErrorMessage ?? Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        jsonProperty.SetValue(target, convertedValue);

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
        if (!TryGetJsonProperty(target, segment, options, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.CanRead)
        {
            errorMessage = Resources.FormatCannotReadProperty(segment);
            return false;
        }

        if (!ConversionResultProvider.TryConvertTo(value, jsonProperty.PropertyType, options, out var convertedValue, out var conversionErrorMessage))
        {
            errorMessage = conversionErrorMessage ?? Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        var currentValue = jsonProperty.GetValue(target);

        // all numeric values are handled as decimals
        if (currentValue is decimal)
        {
            if (!decimal.Equals(currentValue, convertedValue))
            {
                errorMessage = Resources.FormatValueNotEqualToTestValue(JsonSerializer.SerializeToNode(currentValue)?.ToString(), value, segment);
                return false;
            }
        }
        else
        {
            if (!string.Equals(JsonSerializer.SerializeToNode(currentValue)?.ToString(), JsonSerializer.SerializeToNode(convertedValue)?.ToString(), StringComparison.Ordinal))
            {
                errorMessage = Resources.FormatValueNotEqualToTestValue(JsonSerializer.SerializeToNode(currentValue)?.ToString(), value, segment);
                return false;
            }
        }

        errorMessage = null;
        return true;
    }

    public bool TryTraverse(
        object target,
        string segment,
        JsonSerializerOptions options,
        out object value,
        out string errorMessage)
    {
        if (target == null)
        {
            value = null;
            errorMessage = null;
            return false;
        }

        if (TryGetJsonProperty(target, segment, options, out var jsonProperty))
        {
            value = jsonProperty.GetValue(target);
            errorMessage = null;
            return true;
        }

        value = null;
        errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
        return false;
    }

    protected bool TryGetJsonProperty(
        object target,
        string segment,
        JsonSerializerOptions options,
        out IPropertyProxy jsonProperty
    )
    {
        jsonProperty = FindPropertyByName(segment, target, options);

        return jsonProperty != null;
    }

    private static IPropertyProxy FindPropertyByName(string name, object target, JsonSerializerOptions options)
    {
        var propertyName = options.PropertyNamingPolicy != null ? options.PropertyNamingPolicy.ConvertName(name) : name;

        if (target is DynamicObject dyObj)
        {
            return new DynamicObjectPropertyProxy(dyObj, propertyName);
        }
        
        if (target is IDictionary dictionary)
        {
            return new DictionaryPropertyProxy(dictionary, propertyName);
        }

        if (target is IDictionary<string, object> typedDictionary)
        {
            return new DictionaryTypedPropertyProxy(typedDictionary, propertyName);
        }

        if (target is JsonNode jsonElemnt)
        {
            return new JsonNodeProxy(jsonElemnt, propertyName);
        }


        return PropertyProxyCache.GetPropertyProxy(target.GetType(), propertyName);
    }
}
