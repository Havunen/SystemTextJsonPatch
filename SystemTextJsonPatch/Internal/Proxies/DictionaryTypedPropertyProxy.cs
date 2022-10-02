using System;
using System.Collections.Generic;

namespace SystemTextJsonPatch.Internal.Proxies
{
    internal sealed class DictionaryTypedPropertyProxy : IPropertyProxy
    {
        private readonly IDictionary<string, object?> _dictionary;
        private readonly string _propertyName;

        internal DictionaryTypedPropertyProxy(IDictionary<string, object?> dictionary, string propertyName)
        {
            _dictionary = dictionary;
            _propertyName = propertyName;
        }

        public object? GetValue(object target)
        {
            var value = _dictionary[_propertyName];

            return value;
        }

        public void SetValue(object target, object? convertedValue)
        {
            if (_dictionary.ContainsKey(_propertyName))
            {
                _dictionary[_propertyName] = convertedValue;
            }
            else
            {
                _dictionary.Add(_propertyName, convertedValue);
            }
        }

        public void RemoveValue(object target)
        {
            _dictionary.Remove(_propertyName);
        }

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType
        {
            get
            {
                _dictionary.TryGetValue(_propertyName, out var val);
                if (val == null)
                {
                    return typeof(object);
                }

                return val.GetType();
            }
        }
    }
}
