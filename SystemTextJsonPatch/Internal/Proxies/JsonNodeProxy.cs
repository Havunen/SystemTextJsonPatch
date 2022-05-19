using System;
using System.Text.Json.Nodes;

namespace SystemTextJsonPatch.Internal.Proxies
{
    internal sealed class JsonNodeProxy : IPropertyProxy
    {
        private readonly JsonNode _jsonNode;
        private readonly string _propertyName;
        private Type _propertyType;

        internal JsonNodeProxy(JsonNode jsonNode, string propertyName)
        {
            _jsonNode = jsonNode;
            _propertyName = propertyName;
        }

        public object GetValue(object target)
        {
            var value = _jsonNode[this._propertyName];

            _propertyType = value?.GetType() ?? typeof(object);

            return value;
        }

        public void SetValue(object target, object convertedValue)
        {

            if (convertedValue != null)
            {
                var ser = System.Text.Json.JsonSerializer.SerializeToNode(convertedValue);

                if (_jsonNode is JsonArray jsonArray)
                {
                    jsonArray.Add(ser);
                }
                else
                {
                    // ??
                }
            }
            else
            {
                _jsonNode[this._propertyName] = null;
            }

            _propertyType = convertedValue?.GetType() ?? typeof(object);
        }

        //private static Type ConvertJsonElemenType(JsonValueKind type)
        //{
        //    switch (type)
        //    {
        //        case JsonValueKind.String:
        //            return typeof(string);
        //        case JsonValueKind.Number:
        //            return typeof(decimal?);
        //        case JsonValueKind.Object:
        //        case JsonValueKind.Undefined:
        //        case JsonValueKind.Null:
        //            return typeof(object);
        //        case JsonValueKind.Array:
        //            return typeof(ICollection);
        //        case JsonValueKind.True:
        //        case JsonValueKind.False:
        //            return typeof(bool);
        //        default:
        //            return typeof(object);
        //    }
        //}

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType => _propertyType != null ? GetValue(null)?.GetType() ?? typeof(object) : typeof(object);
    }
}
