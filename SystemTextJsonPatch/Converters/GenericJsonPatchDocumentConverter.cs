using System;
using System.Text.Json;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Converters;

public class GenericJsonPatchDocumentConverter<TModel> : JsonPatchDocumentConverterBase<JsonPatchDocument<TModel>, Operation<TModel>>
    where TModel : class
{

    public override JsonPatchDocument<TModel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
        }

        var operations = this.ParseOperations(ref reader, options.PropertyNameCaseInsensitive);

        if (operations == null)
        {
            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
        }

        return new JsonPatchDocument<TModel>(operations, options);
    }

}
