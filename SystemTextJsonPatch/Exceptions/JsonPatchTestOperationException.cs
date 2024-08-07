using System;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch.Exceptions
{
	[Serializable]
#pragma warning disable CA1032 // Implement standard exception constructors
	public sealed class JsonPatchTestOperationException : JsonPatchException
#pragma warning restore CA1032 // Implement standard exception constructors
	{
		public Operation? FailedOperation { get; }
		public object? AffectedObject { get; }

		public JsonPatchTestOperationException(JsonPatchError jsonPatchError) : base(jsonPatchError.ErrorMessage, null)
		{
			FailedOperation = jsonPatchError.Operation;
			AffectedObject = jsonPatchError.AffectedObject;
		}
	}
}
