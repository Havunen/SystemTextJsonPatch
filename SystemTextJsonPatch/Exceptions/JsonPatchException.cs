using System;
using System.Text.Json;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Exceptions;

#pragma warning disable CA1032 // Implement standard exception constructors
[Serializable]
public class JsonPatchException : JsonException
#pragma warning restore CA1032 // Implement standard exception constructors
{
	public Operation? FailedOperation { get; }
	public object? AffectedObject { get; }

	public JsonPatchException(string message, Exception? innerException) : base(message, innerException)
	{
	}

	public JsonPatchException(JsonPatchError jsonPatchError) : this(jsonPatchError.ErrorMessage, null)
	{
		FailedOperation = jsonPatchError.Operation;
		AffectedObject = jsonPatchError.AffectedObject;
	}
}
