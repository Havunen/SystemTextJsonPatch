using System;
using System.Collections.Generic;

namespace SystemTextJsonPatch.Internal.Proxies
{
    internal sealed class DictionaryTypedPropertyProxy : IPropertyProxy
    {
        private readonly IDictionary<string, object> _dictionary;
        private readonly string _propertyName;
        private Type _propertyType;

        internal DictionaryTypedPropertyProxy(IDictionary<string, object> dictionary, string propertyName)
        {
            _dictionary = dictionary;
            _propertyName = propertyName;
        }

        public object GetValue(object target)
        {
            var value = _dictionary[_propertyName];

            _propertyType = value?.GetType() ?? typeof(object);

            return value;
        }

        public void SetValue(object target, object convertedValue)
        {
            if (convertedValue == null)
            {
                _dictionary.Remove(_propertyName);
            }
            else
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
            
            _propertyType = convertedValue?.GetType() ?? typeof(object);
        }

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType => _propertyType != null ? GetValue(null)?.GetType() : typeof(object);
    }
}
