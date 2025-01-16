using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Adapters;
using SystemTextJsonPatch.Converters;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Internal;
using SystemTextJsonPatch.Operations;

namespace SystemTextJsonPatch;

// Implementation details: the purpose of this type of patch document is to ensure we can do type-checking
// when producing a JsonPatchDocument.  However, we cannot send this "typed" over the wire, as that would require
// including type data in the JsonPatchDocument serialized as JSON (to allow for correct deserialization) - that's
// not according to RFC 6902, and would thus break cross-platform compatibility.
[JsonConverter(typeof(JsonPatchDocumentConverterFactory))]
public class JsonPatchDocument<TModel> : IJsonPatchDocument where TModel : class
{
	public List<Operation<TModel>> Operations { get; }

	[JsonIgnore]
	public JsonSerializerOptions Options { get; set; }

	public JsonPatchDocument()
	{
		Operations = new List<Operation<TModel>>();
		Options = new JsonSerializerOptions();
	}

	// Create from list of operations
	public JsonPatchDocument(List<Operation<TModel>> operations, JsonSerializerOptions options)
	{
		ExceptionHelper.ThrowIfNull(operations, nameof(operations));
		ExceptionHelper.ThrowIfNull(options, nameof(options));

		Operations = operations;
		Options = options;
	}

	/// <summary>
	/// Add operation.  Will result in, for example,
	/// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("add", GetPath(path, null), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Add value to list at given position
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <param name="position">position</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value, int position)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("add", GetPath(path, position.ToString(CultureInfo.InvariantCulture)), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Add value to the end of the list
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("add", GetPath(path, Consts.LastElement), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Remove value at target location.  Will result in, for example,
	/// { "op": "remove", "path": "/a/b/c" }
	/// </summary>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, TProp>> path)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("remove", GetPath(path, null), from: null));

		return this;
	}

	/// <summary>
	/// Remove value from list at given position
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="position">position</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, IList<TProp>>> path, int position)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("remove", GetPath(path, position.ToString(CultureInfo.InvariantCulture)), from: null));

		return this;
	}

	/// <summary>
	/// Remove value from end of list
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, IList<TProp>>> path)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("remove", GetPath(path, Consts.LastElement), from: null));

		return this;
	}

	/// <summary>
	/// Replace value.  Will result in, for example,
	/// { "op": "replace", "path": "/a/b/c", "value": 42 }
	/// </summary>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("replace", GetPath(path, null), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Replace value in a list at given position
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <param name="position">position</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value, int position)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("replace", GetPath(path, position.ToString(CultureInfo.InvariantCulture)), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Replace value at end of a list
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("replace", GetPath(path, Consts.LastElement), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Test value.  Will result in, for example,
	/// { "op": "test", "path": "/a/b/c", "value": 42 }
	/// </summary>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("test", GetPath(path, null), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Test value in a list at given position
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <param name="position">position</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value, int position)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("test", GetPath(path, position.ToString(CultureInfo.InvariantCulture)), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Test value at end of a list
	/// </summary>
	/// <typeparam name="TProp">value type</typeparam>
	/// <param name="path">target location</param>
	/// <param name="value">value</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value)
	{
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("test", GetPath(path, Consts.LastElement), from: null, value: value));

		return this;
	}

	/// <summary>
	/// Removes value at specified location and add it to the target location.  Will result in, for example:
	/// { "op": "move", "from": "/a/b/c", "path": "/a/b/d" }
	/// </summary>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, TProp>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("move", GetPath(path, null), GetPath(from, null)));

		return this;
	}

	/// <summary>
	/// Move from a position in a list to a new location
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="positionFrom">position</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>>> from, int positionFrom, Expression<Func<TModel, TProp>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("move", GetPath(path, null), GetPath(from, positionFrom.ToString(CultureInfo.InvariantCulture))));

		return this;
	}

	/// <summary>
	/// Move from a property to a location in a list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <param name="positionTo">position</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>>> path, int positionTo)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("move", GetPath(path, positionTo.ToString(CultureInfo.InvariantCulture)), GetPath(from, null)));

		return this;
	}

	/// <summary>
	/// Move from a position in a list to another location in a list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="positionFrom">position (source)</param>
	/// <param name="path">target location</param>
	/// <param name="positionTo">position (target)</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>>> from, int positionFrom, Expression<Func<TModel, IList<TProp>>> path,
		int positionTo)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("move", GetPath(path, positionTo.ToString(CultureInfo.InvariantCulture)),
			GetPath(from, positionFrom.ToString(CultureInfo.InvariantCulture))));

		return this;
	}

	/// <summary>
	/// Move from a position in a list to the end of another list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="positionFrom">position</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>>> from, int positionFrom, Expression<Func<TModel, IList<TProp>>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("move", GetPath(path, Consts.LastElement), GetPath(from, positionFrom.ToString(CultureInfo.InvariantCulture))));

		return this;
	}

	/// <summary>
	/// Move to the end of a list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("move", GetPath(path, Consts.LastElement), GetPath(from, null)));

		return this;
	}

	/// <summary>
	/// Copy the value at specified location to the target location.  Will result in, for example:
	/// { "op": "copy", "from": "/a/b/c", "path": "/a/b/e" }
	/// </summary>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, TProp>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("copy", GetPath(path, null), GetPath(from, null)));

		return this;
	}

	/// <summary>
	/// Copy from a position in a list to a new location
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="positionFrom">position</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>>> from, int positionFrom, Expression<Func<TModel, TProp>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("copy", GetPath(path, null), GetPath(from, positionFrom.ToString(CultureInfo.InvariantCulture))));

		return this;
	}

	/// <summary>
	/// Copy from a property to a location in a list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <param name="positionTo">position</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>>> path, int positionTo)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("copy", GetPath(path, positionTo.ToString(CultureInfo.InvariantCulture)), GetPath(from, null)));

		return this;
	}

	/// <summary>
	/// Copy from a position in a list to a new location in a list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="positionFrom">position (source)</param>
	/// <param name="path">target location</param>
	/// <param name="positionTo">position (target)</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>>> from, int positionFrom, Expression<Func<TModel, IList<TProp>>> path,
		int positionTo)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("copy", GetPath(path, positionTo.ToString(CultureInfo.InvariantCulture)),
			GetPath(from, positionFrom.ToString(CultureInfo.InvariantCulture))));

		return this;
	}

	/// <summary>
	/// Copy from a position in a list to the end of another list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="positionFrom">position</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>>> from, int positionFrom, Expression<Func<TModel, IList<TProp>>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("copy", GetPath(path, Consts.LastElement), GetPath(from, positionFrom.ToString(CultureInfo.InvariantCulture))));

		return this;
	}

	/// <summary>
	/// Copy to the end of a list
	/// </summary>
	/// <typeparam name="TProp"></typeparam>
	/// <param name="from">source location</param>
	/// <param name="path">target location</param>
	/// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
	public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>>> path)
	{
		ExceptionHelper.ThrowIfNull(from, nameof(from));
		ExceptionHelper.ThrowIfNull(path, nameof(path));

		Operations.Add(new Operation<TModel>("copy", GetPath(path, Consts.LastElement), GetPath(from, null)));

		return this;
	}

	/// <summary>
	/// Apply this JsonPatchDocument
	/// </summary>
	/// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
	public void ApplyTo(TModel objectToApplyTo)
	{
		ExceptionHelper.ThrowIfNull(objectToApplyTo, nameof(objectToApplyTo));

		ApplyTo(objectToApplyTo, new ObjectAdapter(Options, null, AdapterFactory.Default));
	}

	/// <summary>
	/// Apply this JsonPatchDocument
	/// </summary>
	/// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
	/// <param name="logErrorAction">Action to log errors</param>
	public void ApplyTo(TModel objectToApplyTo, Action<JsonPatchError> logErrorAction)
	{
		ApplyTo(objectToApplyTo, new ObjectAdapter(Options, logErrorAction, AdapterFactory.Default), logErrorAction);
	}

	/// <summary>
	/// Apply this JsonPatchDocument
	/// </summary>
	/// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
	/// <param name="adapter">IObjectAdapter instance to use when applying</param>
	/// <param name="logErrorAction">Action to log errors</param>
	public void ApplyTo(TModel objectToApplyTo, IObjectAdapter adapter, Action<JsonPatchError>? logErrorAction)
	{
		ExceptionHelper.ThrowIfNull(objectToApplyTo, nameof(objectToApplyTo));
		ExceptionHelper.ThrowIfNull(adapter, nameof(adapter));

		foreach (var op in Operations)
		{
			try
			{
				op.Apply(objectToApplyTo, adapter);
			}
			catch (JsonException jsonPatchException)
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
	public void ApplyTo(TModel objectToApplyTo, IObjectAdapter adapter)
	{
		ExceptionHelper.ThrowIfNull(objectToApplyTo, nameof(objectToApplyTo));
		ExceptionHelper.ThrowIfNull(adapter, nameof(adapter));

		// apply each operation in order
		foreach (var op in Operations)
		{
			op.Apply(objectToApplyTo, adapter);
		}
	}

	public IList<Operation> GetOperations()
	{
		var allOps = new List<Operation>(Operations?.Count ?? 0);

		if (Operations != null)
		{
			foreach (var op in Operations)
			{
				var untypedOp = new Operation
				{
					Op = op.Op,
					Value = op.Value,
					Path = op.Path,
					From = op.From
				};

				allOps.Add(untypedOp);
			}
		}

		return allOps;
	}

	// Internal for testing
	internal string GetPath<TProp>(Expression<Func<TModel, TProp>> expr, string? position)
	{
		var segments = GetPathSegments(expr.Body);
		var path = string.Join("/", segments);
		if (position != null)
		{
			path += "/" + position;
			if (segments.Count == 0)
			{
				return path;
			}
		}

		return "/" + path;
	}

	private List<string> GetPathSegments(Expression expr)
	{
		var listOfSegments = new List<string>();
		switch (expr.NodeType)
		{
			case ExpressionType.ArrayIndex:
				var binaryExpression = (BinaryExpression)expr;
				listOfSegments.AddRange(GetPathSegments(binaryExpression.Left));
				listOfSegments.Add(binaryExpression.Right.ToString());
				return listOfSegments;

			case ExpressionType.Call:
				var methodCallExpression = (MethodCallExpression)expr;
				listOfSegments.AddRange(GetPathSegments(methodCallExpression.Object));
				listOfSegments.Add(EvaluateExpression(methodCallExpression.Arguments[0]));
				return listOfSegments;

			case ExpressionType.Convert:
				listOfSegments.AddRange(GetPathSegments(((UnaryExpression)expr).Operand));
				return listOfSegments;

			case ExpressionType.MemberAccess:
				var memberExpression = (MemberExpression)expr;
				listOfSegments.AddRange(GetPathSegments(memberExpression.Expression));
				// Get property name, respecting JsonProperty attribute
				listOfSegments.Add(this.GetPropertyNameFromMemberExpression(memberExpression));
				return listOfSegments;

			case ExpressionType.Parameter:
				// Fits "x => x" (the whole document which is "" as JSON pointer)
				return listOfSegments;

			default:
				ExceptionHelper.ThrowJsonPatchException(Resources.FormatExpressionTypeNotSupported(expr));
				return listOfSegments;
		}
	}

	private string GetPropertyNameFromMemberExpression(MemberExpression memberExpression)
	{
		var jsonPropertyNameAttr = memberExpression.Member.GetCustomAttribute<JsonPropertyNameAttribute>();

		if (jsonPropertyNameAttr != null && !string.IsNullOrEmpty(jsonPropertyNameAttr.Name))
		{
			return jsonPropertyNameAttr.Name;
		}

		var memberName = memberExpression.Member.Name;

		if (this.Options.PropertyNamingPolicy != null)
		{
			return this.Options.PropertyNamingPolicy.ConvertName(memberName);
		}

		return memberName;
	}


	// Evaluates the value of the key or index which may be an int or a string,
	// or some other expression type.
	// The expression is converted to a delegate and the result of executing the delegate is returned as a string.
	private static string EvaluateExpression(Expression expression)
	{
		var converted = Expression.Convert(expression, typeof(object));
		var fakeParameter = Expression.Parameter(typeof(object), null);
		var lambda = Expression.Lambda<Func<object, object>>(converted, fakeParameter);
		var func = lambda.Compile();

		return Convert.ToString(func(null), CultureInfo.InvariantCulture);
	}
}
