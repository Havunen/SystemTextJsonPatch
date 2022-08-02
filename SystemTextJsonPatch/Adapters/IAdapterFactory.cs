using System.Text.Json;
using SystemTextJsonPatch.Internal;

namespace SystemTextJsonPatch.Adapters;

/// <summary>
/// Defines the operations used for loading an <see cref="IAdapter"/> based on the current object and ContractResolver.
/// </summary>
public interface IAdapterFactory
{
    /// <summary>
    /// Creates an <see cref="IAdapter"/> for the current object
    /// </summary>
    /// <param name="target">The target object</param>
    /// <returns>The needed <see cref="IAdapter"/></returns>
#pragma warning disable PUB0001
    IAdapter Create(object target, JsonSerializerOptions options);
#pragma warning restore PUB0001
}
