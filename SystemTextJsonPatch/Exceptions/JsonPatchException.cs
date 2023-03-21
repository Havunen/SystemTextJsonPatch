using System;
using System.Text.Json;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Exceptions;

#pragma warning disable CA1032 // Implement standard exception constructors
[Serializable]
public class JsonPatchException : JsonException
#pragma warning restore CA1032 // Implement standard exception constructors
{
    public Operation FailedOperation { get; }
    public object AffectedObject { get; }

    public JsonPatchException(JsonPatchError jsonPatchError, Exception? innerException)
        : base(jsonPatchError.ErrorMessage, innerException)
    {
        FailedOperation = jsonPatchError.Operation;
        AffectedObject = jsonPatchError.AffectedObject;
    }

    public JsonPatchException(JsonPatchError jsonPatchError)
      : this(jsonPatchError, null)
    {
    }

    public JsonPatchException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected JsonPatchException(
        System.Runtime.Serialization.SerializationInfo serializationInfo,
        System.Runtime.Serialization.StreamingContext streamingContext
    ) : base(serializationInfo, streamingContext)
    {
    }
}
