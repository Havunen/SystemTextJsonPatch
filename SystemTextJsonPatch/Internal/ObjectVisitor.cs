using System.Text.Json;
using SystemTextJsonPatch.Adapters;
using SystemTextJsonPatch.Exceptions;

namespace SystemTextJsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class ObjectVisitor
{
	private readonly IAdapterFactory _adapterFactory;
	private readonly JsonSerializerOptions _options;
	private readonly ParsedPath _path;

	public ObjectVisitor(ParsedPath path, JsonSerializerOptions options) : this(path, options, AdapterFactory.Default)
	{
	}

	public ObjectVisitor(ParsedPath path, JsonSerializerOptions options, IAdapterFactory adapterFactory)
	{
		ExceptionHelper.ThrowIfNull(options, nameof(options));
		ExceptionHelper.ThrowIfNull(adapterFactory, nameof(adapterFactory));

		_path = path;
		_options = options;
		_adapterFactory = adapterFactory;
	}

	public bool TryVisit(ref object target, out IAdapter? adapter, out string? errorMessage)
	{
		if (target == null)
		{
			adapter = null;
			errorMessage = null;
			return false;
		}

		adapter = SelectAdapter(target);

		// Traverse until the penultimate segment to get the target object and adapter
		for (var i = 0; i < _path.Segments.Count - 1; i++)
		{
			if (!adapter.TryTraverse(target, _path.Segments[i], _options, out var val, out errorMessage))
			{
				adapter = null;
				return false;
			}

			// If we hit a null on an interior segment then we need to stop traversing.
			if (val == null)
			{
				adapter = null;
				return false;
			}

			target = val;
			adapter = SelectAdapter(target);
		}

		errorMessage = null;
		return true;
	}

	private IAdapter SelectAdapter(object targetObject)
	{
		return _adapterFactory.Create(targetObject, _options);
	}
}
