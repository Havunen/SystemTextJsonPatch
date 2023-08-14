using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Internal;

namespace SystemTextJsonPatch.Adapters;

/// <summary>
/// The default AdapterFactory to be used for resolving <see cref="IAdapter"/>.
/// </summary>
public class AdapterFactory : IAdapterFactory
{
	internal static AdapterFactory Default { get; } = new();

	/// <inheritdoc />
#pragma warning disable PUB0001
	public IAdapter Create(object target)
#pragma warning restore PUB0001
	{
		ExceptionHelper.ThrowIfNull(target, nameof(target));

		return target switch
		{
			JsonObject => new JSonObjectAdapter(),
			IList => new ListAdapter(),
			_ => new PocoAdapter()
		};
	}
}
