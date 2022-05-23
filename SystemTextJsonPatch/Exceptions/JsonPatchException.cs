using System;
using System.Text.Json;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Exceptions;

public class JsonPatchException : JsonException
{
    public Operation FailedOperation { get; }
    public object AffectedObject { get; }

    public JsonPatchException(JsonPatchError jsonPatchError, Exception innerException)
        : base(jsonPatchError.ErrorMessage, innerException)
    {
        FailedOperation = jsonPatchError.Operation;
        AffectedObject = jsonPatchError.AffectedObject;
    }

    public JsonPatchException(JsonPatchError jsonPatchError)
      : this(jsonPatchError, null)
    {
    }

    public JsonPatchException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
