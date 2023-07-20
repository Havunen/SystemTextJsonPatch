using System.Text.Json;
using SystemTextJsonPatch.Internal;

namespace SystemTextJsonPatch.Adapters;

public interface IAdapterFactory
{
	/// <summary>
	/// Creates an <see cref="IAdapter"/> for the current object
	/// </summary>
	/// <param name="target">The target object</param>
	/// <param name="options">Json serializer options</param>
	/// <returns>The needed <see cref="IAdapter"/></returns>
#pragma warning disable PUB0001
	IAdapter Create(object target, JsonSerializerOptions options);
#pragma warning restore PUB0001
}
