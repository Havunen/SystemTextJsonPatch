using System;
using System.Diagnostics.CodeAnalysis;

namespace SystemTextJsonPatch.Exceptions
{
	internal class ExceptionHelper
	{
		internal static void ThrowIfNull(
#if !NETSTANDARD2_0
			[NotNull] 
#endif
			object? argument,
			string paramName
		)
		{
#if NETSTANDARD2_0
			if (argument == null)
			{
				throw new ArgumentNullException(nameof(argument));
			}
#else
			ArgumentNullException.ThrowIfNull(argument, paramName);
#endif
		}

#if !NETSTANDARD2_0
		[DoesNotReturn]
#endif
		internal static void ThrowJsonPatchException(string message)
		{
			throw new JsonPatchException(message, null);
		}


#if !NETSTANDARD2_0
		[DoesNotReturn]
#endif
		internal static void ThrowJsonPatchTestException(JsonPatchError jsonPatchError)
		{
			throw new JsonPatchTestOperationException(jsonPatchError);
		}
	}
}
