using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonPatch.Converters
{
    public class JsonPatchDocumentConverterFactory : JsonConverterFactory
    {
        private static readonly Type GenericJsonPatchDocumentType = typeof(JsonPatchDocument<>);
        private static readonly Type JsonPatchDocumentType = typeof(JsonPatchDocument);

        /// <inheritdoc />
        public sealed override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == JsonPatchDocumentType || (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == GenericJsonPatchDocumentType);
        }

        /// <inheritdoc />
        public sealed override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == JsonPatchDocumentType)
            {
                return new JsonPatchDocumentConverter();
            }


            return (JsonConverter)Activator.CreateInstance(
                typeof(GenericJsonPatchDocumentConverter<>).MakeGenericType(typeToConvert.GenericTypeArguments[0]),
                Array.Empty<object>()
            )!;
        }
    }
}
