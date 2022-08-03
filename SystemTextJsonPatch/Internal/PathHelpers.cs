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

        if (path.Contains("//", StringComparison.Ordinal))
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
