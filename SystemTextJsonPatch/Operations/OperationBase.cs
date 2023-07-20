using System;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Exceptions;

namespace SystemTextJsonPatch.Operations;

public abstract class OperationBase
{
    private string? _op;
    private OperationType _operationType;

    [JsonIgnore]
    public OperationType OperationType => _operationType;

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("op")]
    public string? Op
    {
        get => _op;
        set
        {
            OperationType result;
            if (!Enum.TryParse<OperationType>(value, ignoreCase: true, result: out result))
            {
                result = OperationType.Invalid;
            }
            _operationType = result;
            _op = value;
        }
    }

    [JsonPropertyName("from")]
    public string? From { get; set; }

    protected OperationBase()
    {
    }

    protected OperationBase(string op, string path, string? from)
    {
	    ExceptionHelper.ThrowIfNull(op, nameof(op));
	    ExceptionHelper.ThrowIfNull(path, nameof(path));

		this.Op = op;
        this.Path = path;
        this.From = from;
    }
}
