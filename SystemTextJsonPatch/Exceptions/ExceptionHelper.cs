using System;

namespace SystemTextJsonPatch.Exceptions
{
	internal class ExceptionHelper
	{
		public static void ThrowIfNull(object? argument, string paramName)
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
	}
}
