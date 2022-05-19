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
        public sealed override JsonConverter CreateConverter(Type patchDocumentType, JsonSerializerOptions options)
        {
            if (patchDocumentType == JsonPatchDocumentType)
            {
                return new JsonPatchDocumentConverter();
            }


            return (JsonConverter)Activator.CreateInstance(
                typeof(GenericJsonPatchDocumentConverter<>).MakeGenericType(patchDocumentType.GenericTypeArguments[0]),
                new object[] { }
            );
        }
    }
}
