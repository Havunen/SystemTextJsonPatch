﻿using System.Text.Json;
using SystemTextJsonPatch.Internal;

namespace SystemTextJsonPatch.Adapters;

public interface IAdapterFactory
{
	/// <summary>
	/// Creates an <see cref="IAdapter"/> for the current object
	/// </summary>
	/// <param name="target">The target object</param>
	/// <returns>The needed <see cref="IAdapter"/></returns>
#pragma warning disable PUB0001
	IAdapter Create(object target);
#pragma warning restore PUB0001
}
