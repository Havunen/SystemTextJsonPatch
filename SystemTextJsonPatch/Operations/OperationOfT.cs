using System;
using SystemTextJsonPatch.Adapters;
using SystemTextJsonPatch.Exceptions;

namespace SystemTextJsonPatch.Operations;

public class Operation<TModel> : Operation where TModel : class
{
	public Operation()
	{
	}

	public Operation(string op, string path, string? from, object? value)
		: base(op, path, from)
	{
		this.Value = value;
	}

	public Operation(string op, string path, string? from)
		: base(op, path, from)
	{
	}

	public void Apply(TModel objectToApplyTo, IObjectAdapter adapter)
	{
		ExceptionHelper.ThrowIfNull(objectToApplyTo, nameof(objectToApplyTo));
		ExceptionHelper.ThrowIfNull(adapter, nameof(adapter));

		switch (OperationType)
		{
			case OperationType.Add:
				adapter.Add(this, objectToApplyTo);
				break;
			case OperationType.Remove:
				adapter.Remove(this, objectToApplyTo);
				break;
			case OperationType.Replace:
				adapter.Replace(this, objectToApplyTo);
				break;
			case OperationType.Move:
				adapter.Move(this, objectToApplyTo);
				break;
			case OperationType.Copy:
				adapter.Copy(this, objectToApplyTo);
				break;
			case OperationType.Test:
				if (adapter is IObjectAdapterWithTest adapterWithTest)
				{
					adapterWithTest.Test(this, objectToApplyTo);
					break;
				}

				throw new JsonPatchException(new JsonPatchError(objectToApplyTo, this, Resources.TestOperationNotSupported));
			case OperationType.Invalid:
				throw new JsonPatchException(
					Resources.FormatInvalidJsonPatchOperation(Op), innerException: null);
			default:
				break;
		}
	}
}
