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
            if (JsonDocument.TryParseValue(ref reader, out JsonDocument doc))
            {
                var operations = new List<TOperation>();

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var jsonEl in doc.RootElement.EnumerateArray())
                    {
                        if (jsonEl.ValueKind != JsonValueKind.Object)
                        {
                            throw new JsonPatchException(Resources.InvalidJsonPatchDocument, null);
                        }

                        string op = null;
                        string path = null;
                        string from = null;
                        object val = null;

                        foreach (var jsonObj in jsonEl.EnumerateObject())
                        {
                            if ("op".Equals(jsonObj.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                op = jsonObj.Value.GetString();
                            }
                            else if ("path".Equals(jsonObj.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                path = jsonObj.Value.GetString();
                            }
                            else if ("from".Equals(jsonObj.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                from = jsonObj.Value.GetString();
                            }
                            else if ("value".Equals(jsonObj.Name, StringComparison.OrdinalIgnoreCase))
                            {

                                switch (jsonObj.Value.ValueKind)
                                {
                                    case JsonValueKind.String:
                                        val = jsonObj.Value.GetString();
                                        break;
                                    case JsonValueKind.Number:
                                        val = jsonObj.Value.GetDecimal();
                                        break;
                                    case JsonValueKind.True:
                                    case JsonValueKind.False:
                                        val = jsonObj.Value.GetBoolean();
                                        break;
                                    case JsonValueKind.Null:
                                        val = null;
                                        break;
                                    default:
                                        val = jsonObj.Value;
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
