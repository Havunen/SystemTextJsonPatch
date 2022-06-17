using System;
using SystemTextJsonPatch.Exceptions;

namespace SystemTextJsonPatch.Internal;

internal static class ErrorReporter
{
    public static readonly Action<JsonPatchError> Default = error => throw new JsonPatchException(error);
}
