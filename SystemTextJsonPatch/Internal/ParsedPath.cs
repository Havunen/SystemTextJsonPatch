using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SystemTextJsonPatch.Exceptions;

namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public readonly struct ParsedPath : IEquatable<ParsedPath>
{
    private readonly string[] _segments;

    public ParsedPath(string? path)
    {
	    ExceptionHelper.ThrowIfNull(path, nameof(path));

	    _segments = ParsePath(path!);
    }

    public string? LastSegment
    {
        get
        {
            if (_segments.Length == 0)
            {
                return null;
            }

            return _segments[_segments.Length - 1];
        }
    }

    public IReadOnlyList<string> Segments => _segments;

    private static string[] ParsePath(string path)
    {
        var strings = new List<string>();
        var sb = new StringBuilder(path.Length);

        for (var i = 0; i < path.Length; i++)
        {
            var c = path[i];

            if (c == '/')
            {
                if (sb.Length > 0)
                {
                    strings.Add(sb.ToString());
                    sb.Length = 0;
                }
            }
            else if (c == '~')
            {
                ++i;
				if (i >= path.Length)
                {
                    throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
                }

				c = path[i];
				if (c == '0')
                {
                    sb.Append('~');
                }
                else if (c == '1')
                {
                    sb.Append('/');
                }
                else
                {
                    throw new JsonPatchException(Resources.FormatInvalidValueForPath(path), null);
                }
            }
            else
            {
                sb.Append(c);
            }
        }

        if (sb.Length > 0)
        {
            strings.Add(sb.ToString());
        }

        return strings.ToArray();
    }

    public static bool operator ==(ParsedPath left, ParsedPath right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ParsedPath left, ParsedPath right)
    {
        return !(left == right);
    }

	public static bool Equals(ParsedPath left, ParsedPath right)
	{
		return left == right;
	}

	public override bool Equals(object? obj)
	{
        if (obj is ParsedPath parsedPath)
        {
            return this == parsedPath;
        }

        return false;
	}

	public bool Equals(ParsedPath other)
	{
        return this == other;
	}

	public override int GetHashCode()
	{
		return ((IStructuralEquatable)this._segments).GetHashCode(EqualityComparer<string>.Default);
	}
}
