using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public static class ConversionResultProvider
{
    internal static bool TryConvertTo(object? value, Type typeToConvertTo, JsonSerializerOptions options, out object? convertedValue, out string? errorMessage)
    {
        errorMessage = null;

        if (value == null)
        {
            convertedValue = null;
            return IsNullableType(typeToConvertTo);
        }

        if (typeToConvertTo.IsInstanceOfType(value))
        {
            // No need to convert
            convertedValue = value;
            return true;
        }

        if (value is decimal decimalValue)
        {
            if (TryConvertDecimalToNumber(decimalValue, typeToConvertTo, out convertedValue))
            {
                return true;
            }
        }

        try
        {
            convertedValue = Deserialize(value, typeToConvertTo, options);

            return true;
        }
        catch (JsonException jsonException)
        {
            // Do not change application layer JsonException messages, but hide errors from System.Text.Json
            errorMessage = "System.Text.Json".Equals(jsonException.Source, StringComparison.Ordinal)
                ? null
                : jsonException.Message;
            convertedValue = null;

            return false;
        }
        catch
        {
            convertedValue = null;

            return false;
        }
    }

    internal static bool TryCopyTo(object? value, Type typeToConvertTo, JsonSerializerOptions options, out object? convertedValue)
    {
        var targetType = typeToConvertTo;
        if (value == null)
        {
            convertedValue = null;

            return true;
        }

        if (typeToConvertTo.IsInstanceOfType(value))
        {
            // Keep original type
            targetType = value.GetType();
        }

        if (value is decimal decimalValue)
        {
            if (TryConvertDecimalToNumber(decimalValue, targetType, out convertedValue))
            {
                return true;
            }
        }

        if (typeToConvertTo == typeof(string) && value is string stringValue)
        {
            convertedValue = stringValue;
            return true;
        }

        try
        {
            convertedValue = Deserialize(value, typeToConvertTo, options);

            return true;
        }
        catch
        {
            convertedValue = null;

            return false;
        }
    }

    // Because all JSON number fields are parsed as decimal
    // fast-path for converting decimal to target types
    private static bool TryConvertDecimalToNumber(decimal decimalValue, Type typeToConvertTo, out object? convertedValue)
    {
        switch (Type.GetTypeCode(typeToConvertTo))
        {
            case TypeCode.SByte:
                convertedValue = decimal.ToSByte(decimalValue);
                break;
            case TypeCode.Byte:
                convertedValue = decimal.ToByte(decimalValue);
                break;
            case TypeCode.Int16:
                convertedValue = decimal.ToInt16(decimalValue);
                break;
            case TypeCode.UInt16:
                convertedValue = decimal.ToUInt16(decimalValue);
                break;
            case TypeCode.Int32:
                convertedValue = decimal.ToInt32(decimalValue);
                break;
            case TypeCode.UInt32:
                convertedValue = decimal.ToUInt32(decimalValue);
                break;
            case TypeCode.Int64:
                convertedValue = decimal.ToInt64(decimalValue);
                break;
            case TypeCode.UInt64:
                convertedValue = decimal.ToUInt64(decimalValue);
                break;
            case TypeCode.Single:
                convertedValue = decimal.ToSingle(decimalValue);
                break;
            case TypeCode.Double:
                convertedValue = decimal.ToDouble(decimalValue);
                break;
            case TypeCode.Decimal:
                convertedValue = decimalValue;
                break;
            case TypeCode.Empty:
            case TypeCode.Object:
            case TypeCode.DBNull:
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.String:
            default:
                convertedValue = null;
                // fallback to serializer logic
                return false;
        }

        return true;
    }

    private static object? Deserialize(object? value, Type typeToConvertTo, JsonSerializerOptions options)
    {
		if (typeToConvertTo == typeof(JsonElement))
		{
			return JsonSerializer.SerializeToElement(value, options);
		}
		if (typeToConvertTo == typeof(JsonNode))
        {
            return JsonSerializer.SerializeToNode(value, options);
        }
        if (typeToConvertTo == typeof(JsonDocument))
        {
            return JsonSerializer.SerializeToDocument(value, options);
        }

		if (value is JsonElement jsonEl)
		{
			return JsonSerializer.Deserialize(jsonEl, typeToConvertTo, options);
		}
		if (value is JsonNode jsonNode)
		{
			return JsonSerializer.Deserialize(jsonNode, typeToConvertTo, options);
		}
		if (value is JsonDocument jsonDoc)
		{
			return JsonSerializer.Deserialize(jsonDoc, typeToConvertTo, options);
		}

		return JsonSerializer.Deserialize(JsonSerializer.SerializeToUtf8Bytes(value, options), typeToConvertTo, options);
    }

    private static bool IsNullableType(Type type)
    {
        if (type.IsValueType)
        {
            // value types are only nullable if they are Nullable<T>
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        // reference types are always nullable
        return true;
    }
}
