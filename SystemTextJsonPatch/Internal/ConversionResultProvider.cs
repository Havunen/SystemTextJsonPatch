using System;
using System.Text.Json;

namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public static class ConversionResultProvider
{
    public static ConversionResult ConvertTo(object value, Type typeToConvertTo)
    {
        return ConvertTo(value, typeToConvertTo, null);
    }

    internal static ConversionResult ConvertTo(object value, Type typeToConvertTo, JsonSerializerOptions options)
    {
        if (value == null)
        {
            return new ConversionResult(IsNullableType(typeToConvertTo), null);
        }

        if (typeToConvertTo.IsInstanceOfType(value))
        {
            // No need to convert
            return new ConversionResult(true, value);
        }

        try
        {
            if (options == null)
            {
                var deserialized = JsonSerializer.Deserialize(JsonSerializer.Serialize(value), typeToConvertTo);
                return new ConversionResult(true, deserialized);
            }
            else
            {
                var deserialized =
                    JsonSerializer.Deserialize(JsonSerializer.Serialize(value, options), typeToConvertTo, options);

                return new ConversionResult(true, deserialized);
            }
        }
        catch (JsonException jsonException)
        {
            // Do not change application layer JsonException messages, but hide errors from System.Text.Json
            return new ConversionResult(canBeConverted: false, convertedInstance: null, "System.Text.Json".Equals(jsonException.Source, StringComparison.Ordinal) ? null : jsonException.Message);
        }
        catch (Exception)
        {
            return new ConversionResult(canBeConverted: false, convertedInstance: null);
        }
    }

    public static ConversionResult CopyTo(object value, Type typeToConvertTo)
    {
        var targetType = typeToConvertTo;
        if (value == null)
        {
            return new ConversionResult(canBeConverted: true, convertedInstance: null);
        }

        if (typeToConvertTo.IsInstanceOfType(value))
        {
            // Keep original type
            targetType = value.GetType();
        }
        try
        {
            var deserialized = JsonSerializer.Deserialize(JsonSerializer.Serialize(value), targetType);
            return new ConversionResult(true, deserialized);
        }
        catch
        {
            return new ConversionResult(canBeConverted: false, convertedInstance: null);
        }
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
