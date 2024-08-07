using System;
using System.Runtime.CompilerServices;

namespace SystemTextJsonPatch.Internal
{
	internal static class ByteHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool BytesEquals(ReadOnlySpan<byte> span1, ReadOnlySpan<byte> span2)
		{
			if (span1.Length != span2.Length)
			{
				return false;
			}

			return span1.SequenceEqual(span2);
		}
	}
}
