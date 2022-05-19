using System;
using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Converters;

public class JsonPatchDocumentConverter : JsonPatchDocumentConverterBase<JsonPatchDocument, Operation>
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }
    public override bool HandleNull => true;

    public override JsonPatchDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
        }

        var operations = this.ParseOperations(ref reader);

        if (operations == null)
        {
            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
        }

        return new JsonPatchDocument(operations, options);
    }
}
