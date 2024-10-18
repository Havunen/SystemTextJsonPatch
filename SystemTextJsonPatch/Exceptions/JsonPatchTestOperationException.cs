using System;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Exceptions
{
	[Serializable]
#pragma warning disable CA1032 // Implement standard exception constructors
	public sealed class JsonPatchTestOperationException(JsonPatchError jsonPatchError) : JsonPatchException(jsonPatchError)
#pragma warning restore CA1032 // Implement standard exception constructors
	{
	}
}
