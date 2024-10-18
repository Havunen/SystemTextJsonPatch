using System;
using SystemTextJsonPatch.Exceptions;

namespace SystemTextJsonPatch.Internal;

internal static class ErrorReporter
{
	public static readonly Action<JsonPatchError> Default = error => ExceptionHelper.ThrowJsonPatchException(error);
	public static readonly Action<JsonPatchError> TestDefault = error => ExceptionHelper.ThrowJsonPatchTestException(error);
}
