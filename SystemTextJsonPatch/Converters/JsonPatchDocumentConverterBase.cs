using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Converters
{
    public abstract class JsonPatchDocumentConverterBase<TType, TOperation> : JsonConverter<TType>
        where TType : class, IJsonPatchDocument, new()
        where TOperation : Operation, new()


    {
        protected List<TOperation> ParseOperations(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var operations = new List<TOperation>();

                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
                    }

                    string? op = null;
                    string? path = null;
                    string? from = null;
                    object? val = null;

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
                        }

                        var name = reader.GetString();

                        if (!reader.Read())
                        {
                            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
                        }

                        if ("op".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            op = reader.GetString();
                        }
                        else if ("path".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            path = reader.GetString();
                        }
                        else if ("from".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            from = reader.GetString();
                        }
                        else if ("value".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            switch (reader.TokenType)
                            {
                                case JsonTokenType.String:
                                    val = reader.GetString();
                                    break;
                                case JsonTokenType.Number:
                                    val = reader.GetDecimal();
                                    break;
                                case JsonTokenType.True:
                                    val = true;
                                    break;
                                case JsonTokenType.False:
                                    val = false;
                                    break;
                                case JsonTokenType.Null:
                                    val = null;
                                    break;
                                default:
                                    val = JsonElement.ParseValue(ref reader);
                                    break;
                            }
                        }
                        else
                        {
                            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
                        }
                    }

                    var operation = new TOperation
                    {
                        op = op,
                        path = path,
                        from = from,
                        value = val
                    };

                    operations.Add(operation);
                }

                return operations;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, TType value, JsonSerializerOptions options)
        {
            if (value is IJsonPatchDocument jsonPatchDoc)
            {
                var operations = jsonPatchDoc.GetOperations();

                writer.WriteStartArray();

                foreach (var operation in operations)
                {
                    writer.WriteStartObject();
                    writer.WriteString("op", operation.op);
                    writer.WriteString("path", operation.path);

                    if (!string.IsNullOrEmpty(operation.from))
                    {
                        writer.WriteString("from", operation.from);
                    }

                    writer.WritePropertyName("value");
                    JsonSerializer.Serialize(writer, operation.value, options);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }
            else
            {
                throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
            }
        }
    }
}
