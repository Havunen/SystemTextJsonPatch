﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Converters
{
	public abstract class JsonPatchDocumentConverterBase<TType, TOperation> : JsonConverter<TType>
		where TType : class, IJsonPatchDocument, new() where TOperation : Operation, new()
	{
		protected static List<TOperation>? ParseOperations(ref Utf8JsonReader reader, bool caseInSensitive)
		{
			if (reader.TokenType == JsonTokenType.StartArray)
			{
				return caseInSensitive ? ParseOperationCaseInSensitive(ref reader) : ParseOperationCaseSensitive(ref reader);
			}

			return null;
		}

		private static List<TOperation> ParseOperationCaseSensitive(ref Utf8JsonReader reader)
		{
			var operations = new List<TOperation>();

			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				if (reader.TokenType != JsonTokenType.StartObject)
				{
					ThrowJsonPatchException();
				}

				string? op = null;
				string? path = null;
				string? from = null;
				object? val = null;

				while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
				{
					if (reader.TokenType != JsonTokenType.PropertyName)
					{
						ThrowJsonPatchException();
					}

					if (reader.ValueTextEquals("op"u8))
					{
						CheckedRead(ref reader);
						op = reader.GetString();
					}
					else if (reader.ValueTextEquals("path"u8))
					{
						CheckedRead(ref reader);
						path = reader.GetString();
					}
					else if (reader.ValueTextEquals("from"u8))
					{
						CheckedRead(ref reader);
						from = reader.GetString();
					}
					else if (reader.ValueTextEquals("value"u8))
					{
						CheckedRead(ref reader);
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
						ThrowJsonPatchException();
					}
				}

				var operation = new TOperation
				{
					Op = op,
					Path = path,
					From = from,
					Value = val
				};

				operations.Add(operation);
			}

			return operations;
		}

		private static List<TOperation> ParseOperationCaseInSensitive(ref Utf8JsonReader reader)
		{
			var operations = new List<TOperation>();

			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				if (reader.TokenType != JsonTokenType.StartObject)
				{
					ExceptionHelper.ThrowJsonPatchException(Resources.InvalidJsonPatchDocument);
				}

				string? op = null;
				string? path = null;
				string? from = null;
				object? val = null;

				while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
				{
					if (reader.TokenType != JsonTokenType.PropertyName)
					{
						ExceptionHelper.ThrowJsonPatchException(Resources.InvalidJsonPatchDocument);
					}

					var name = reader.GetString();

					if (!reader.Read())
					{
						ExceptionHelper.ThrowJsonPatchException(Resources.InvalidJsonPatchDocument);
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
						ExceptionHelper.ThrowJsonPatchException(Resources.InvalidJsonPatchDocument);
					}
				}

				var operation = new TOperation
				{
					Op = op,
					Path = path,
					From = from,
					Value = val
				};

				operations.Add(operation);
			}

			return operations;
		}

		private static void ThrowJsonPatchException()
		{
			ExceptionHelper.ThrowJsonPatchException(Resources.InvalidJsonPatchDocument);
		}

		private static void CheckedRead(ref Utf8JsonReader reader)
		{
			if (!reader.Read())
			{
				ThrowJsonPatchException();
			}
		}

		public override void Write(Utf8JsonWriter writer, TType value, JsonSerializerOptions options)
		{
			if (value is IJsonPatchDocument jsonPatchDoc)
			{
				var operations = jsonPatchDoc.GetOperations();

				writer!.WriteStartArray();

				foreach (var operation in operations)
				{
					writer.WriteStartObject();
					writer.WriteString("op"u8, operation.Op);
					writer.WriteString("path"u8, operation.Path);

					if (!string.IsNullOrEmpty(operation.From))
					{
						writer.WriteString("from"u8, operation.From);
					}

					if (operation.OperationType is OperationType.Add or OperationType.Replace or OperationType.Test)
					{
						writer.WritePropertyName("value"u8);
						JsonSerializer.Serialize(writer, operation.Value, options);
					}

					writer.WriteEndObject();
				}

				writer.WriteEndArray();
			}
			else
			{
				ExceptionHelper.ThrowJsonPatchException(Resources.InvalidJsonPatchDocument);
			}
		}
	}
}
