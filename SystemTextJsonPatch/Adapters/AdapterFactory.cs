using System.Collections;
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
    public IAdapter Create(object target, JsonSerializerOptions options)
#pragma warning restore PUB0001
    {
        ExceptionHelper.ThrowIfNull(target, nameof(target));

        if (target is JsonObject)
        {
            return new JSonObjectAdapter();
        }
        if (target is IList)
        {
            return new ListAdapter();
        }

        return new PocoAdapter();
    }
}

