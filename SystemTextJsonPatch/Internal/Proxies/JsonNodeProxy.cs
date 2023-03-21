using System;
using System.Text.Json;
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
            _jsonNode[this._propertyName] = convertedValue != null ? JsonSerializer.SerializeToNode(convertedValue) : null;
        }

        public void RemoveValue(object target)
        {
            _jsonNode.AsObject().Remove(_propertyName);
        }

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType => typeof(JsonNode);
    }
}