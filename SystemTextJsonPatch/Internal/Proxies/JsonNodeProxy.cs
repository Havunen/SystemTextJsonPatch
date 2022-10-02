using System;
using System.Globalization;
using System.Text.Json.Nodes;

namespace SystemTextJsonPatch.Internal.Proxies
{
    internal sealed class JsonNodeProxy : IPropertyProxy
    {
        private readonly JsonNode _jsonNode;
        private readonly string _propertyName;

        internal JsonNodeProxy(JsonNode jsonNode, string propertyName)
        {
            _jsonNode = jsonNode;
            _propertyName = propertyName;
        }

        public object? GetValue(object target)
        {
            var value = _jsonNode[this._propertyName];

            return value;
        }

        public void SetValue(object target, object? convertedValue)
        {
            if (convertedValue != null)
            {
                var ser = System.Text.Json.JsonSerializer.SerializeToNode(convertedValue);

                if (_jsonNode is JsonArray jsonArray && _propertyName == "-")
                {
                    jsonArray.Add(ser);
                }
                else
                {
                    _jsonNode[this._propertyName] = ser;
                }
            }
            else
            {
                _jsonNode[this._propertyName] = null;
            }
        }

        public void RemoveValue(object target)
        {
            if (_jsonNode is JsonArray jsonArray)
            {
                if (_propertyName == "-")
                {
                    jsonArray.RemoveAt(jsonArray.Count - 1);
                } else
                {
                    jsonArray.RemoveAt(index: int.Parse(_propertyName, NumberStyles.Number, CultureInfo.InvariantCulture));
                }
            } else
            {
                _jsonNode.AsObject().Remove(_propertyName);
            }
        }

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType => typeof(JsonNode);
    }
}