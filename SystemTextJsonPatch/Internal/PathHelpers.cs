using System;
using SystemTextJsonPatch.Exceptions;

namespace SystemTextJsonPatch.Internal;

internal static class PathHelpers
{
	internal static string ValidateAndNormalizePath(string path)
	{
		// check for most common path errors on create.  This is not
		// absolutely necessary, but it allows us to already catch mistakes
		// on creation of the patch document rather than on execute.

#if NETSTANDARD2_0
		if (path.Contains("//"))
#else
        if (path.Contains("//", StringComparison.Ordinal))
#endif
		{
			throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
		}

		if (!path.StartsWith("/", StringComparison.Ordinal))
		{
			return "/" + path;
		}

		return path;
	}
}
