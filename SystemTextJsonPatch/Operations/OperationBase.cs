using System;
using System.Text.Json.Serialization;

namespace SystemTextJsonPatch.Operations;

public abstract class OperationBase
{
    private string _op;
    private OperationType _operationType;

    [JsonIgnore]
    public OperationType OperationType
    {
        get
        {
            return _operationType;
        }
    }

    [JsonPropertyName("path")]
    public string path { get; set; }

    [JsonPropertyName("op")]
    public string op
    {
        get
        {
            return _op;
        }
        set
        {
            OperationType result;
            if (!Enum.TryParse(value, ignoreCase: true, result: out result))
            {
                result = OperationType.Invalid;
            }
            _operationType = result;
            _op = value;
        }
    }

    [JsonPropertyName("from")]
    public string? from { get; set; }

    public OperationBase()
    {
    }

    public OperationBase(string op, string path, string from)
    {
        if (op == null)
        {
            throw new ArgumentNullException(nameof(op));
        }

        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        this.op = op;
        this.path = path;
        this.from = from;
    }

    public bool ShouldSerializefrom()
    {
        return (OperationType == OperationType.Move
            || OperationType == OperationType.Copy);
    }
}
