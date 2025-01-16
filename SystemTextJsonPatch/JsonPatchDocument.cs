using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Adapters;
using SystemTextJsonPatch.Converters;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Internal;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch;

// Implementation details: the purpose of this type of patch document is to allow creation of such
// documents for cases where there's no class/DTO to work on. Typical use case: backend not built in
// .NET or architecture doesn't contain a shared DTO layer.
[JsonConverter(typeof(JsonPatchDocumentConverter))]
public sealed class JsonPatchDocument : IJsonPatchDocument
{
	public List<Operation> Operations { get; }

	[JsonIgnore]
	public JsonSerializerOptions Options { get; set; }

	public JsonPatchDocument()
	{
		Operations = new List<Operation>();
		Options = new JsonSerializerOptions();
	}

	public JsonPatchDocument(List<Operation> operations, JsonSerializerOptions options)
	{
		ExceptionHelper.ThrowIfNull(operations, nameof(operations));
		ExceptionHelper.ThrowIfNull(options, nameof(options));

		Options = options;
		Operations = operations;
	}

	/// <summary>
	/// Add operation.  Will result in, for example,
	/// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
	/// </summary>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument"/> for chaining.</returns>
	public JsonPatchDocument Add(string path, object? value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation("add", PathHelpers.ValidateAndNormalizePath(path), null, value));
		return this;
	}

	/// <summary>
	/// Remove value at target location.  Will result in, for example,
	/// { "op": "remove", "path": "/a/b/c" }
	/// </summary>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument"/> for chaining.</returns>
	public JsonPatchDocument Remove(string path)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation("remove", PathHelpers.ValidateAndNormalizePath(path), null, null));
		return this;
	}

	/// <summary>
	/// Replace value.  Will result in, for example,
	/// { "op": "replace", "path": "/a/b/c", "value": 42 }
	/// </summary>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument"/> for chaining.</returns>
	public JsonPatchDocument Replace(string path, object? value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation("replace", PathHelpers.ValidateAndNormalizePath(path), null, value));
		return this;
	}

	/// <summary>
	/// Test value.  Will result in, for example,
	/// { "op": "test", "path": "/a/b/c", "value": 42 }
	/// </summary>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument"/> for chaining.</returns>
	public JsonPatchDocument Test(string path, object? value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation("test", PathHelpers.ValidateAndNormalizePath(path), null, value));
		return this;
	}

	/// <summary>
	/// Removes value at specified location and add it to the target location.  Will result in, for example:
	/// { "op": "move", "from": "/a/b/c", "path": "/a/b/d" }
	/// </summary>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument"/> for chaining.</returns>
	public JsonPatchDocument Move(string from, string path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation("move", PathHelpers.ValidateAndNormalizePath(path), PathHelpers.ValidateAndNormalizePath(from)));
		return this;
	}

	/// <summary>
	/// Copy the value at specified location to the target location.  Will result in, for example:
	/// { "op": "copy", "from": "/a/b/c", "path": "/a/b/e" }
	/// </summary>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument"/> for chaining.</returns>
	public JsonPatchDocument Copy(string from, string path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation("copy", PathHelpers.ValidateAndNormalizePath(path), PathHelpers.ValidateAndNormalizePath(from)));
		return this;
	}

	/// <summary>
	/// Apply this JsonPatchDocument
	/// </summary>
	/// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
	public void ApplyTo(object objectToApplyTo)
	{
		ExceptionHelper.ThrowIfNull(objectToApplyTo, nameof(objectToApplyTo));

		ApplyTo(objectToApplyTo, new ObjectAdapter(Options, null, AdapterFactory.Default));
	}

	/// <summary>
	/// Apply this JsonPatchDocument
	/// </summary>
	/// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
	/// <param name="logErrorAction">Action to log errors</param>
	public void ApplyTo(object objectToApplyTo, Action<JsonPatchError> logErrorAction)
	{
		ApplyTo(objectToApplyTo, new ObjectAdapter(Options, logErrorAction, AdapterFactory.Default), logErrorAction);
	}

	/// <summary>
	/// Apply this JsonPatchDocument
	/// </summary>
	/// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
	/// <param name="adapter">IObjectAdapter instance to use when applying</param>
	/// <param name="logErrorAction">Action to log errors</param>
	public void ApplyTo(object objectToApplyTo, IObjectAdapter adapter, Action<JsonPatchError>? logErrorAction)
	{
		ExceptionHelper.ThrowIfNull(objectToApplyTo, nameof(objectToApplyTo));
		ExceptionHelper.ThrowIfNull(adapter, nameof(adapter));

		foreach (var op in Operations)
		{
			try
			{
				op.Apply(objectToApplyTo, adapter);
			}
			catch (JsonPatchException jsonPatchException)
			{
				var errorReporter = logErrorAction ?? ErrorReporter.Default;
				errorReporter(new JsonPatchError(objectToApplyTo, op, jsonPatchException.Message));

				// As per JSON Patch spec if an operation results in error, further operations should not be executed.
				break;
			}
		}
	}

	/// <summary>
	/// Apply this JsonPatchDocument
	/// </summary>
	/// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
	/// <param name="adapter">IObjectAdapter instance to use when applying</param>
	public void ApplyTo(object objectToApplyTo, IObjectAdapter adapter)
	{
		ExceptionHelper.ThrowIfNull(objectToApplyTo, nameof(objectToApplyTo));
		ExceptionHelper.ThrowIfNull(adapter, nameof(adapter));

		// apply each operation in order
		foreach (var op in Operations)
		{
			op.Apply(objectToApplyTo, adapter);
		}
	}

	IList<Operation> IJsonPatchDocument.GetOperations()
	{
		var allOps = new List<Operation>(Operations?.Count ?? 0);

		if (Operations != null)
		{
			foreach (var op in Operations)
			{
				var untypedOp = new Operation();

				untypedOp.Op = op.Op;
				untypedOp.Value = op.Value;
				untypedOp.Path = op.Path;
				untypedOp.From = op.From;

				allOps.Add(untypedOp);
			}
		}

		return allOps;
	}
}
