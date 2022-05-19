using System;
using System.Collections;

namespace SystemTextJsonPatch.Internal.Proxies
{
    internal sealed class DictionaryPropertyProxy : IPropertyProxy
    {
        private readonly IDictionary _dictionary;
        private readonly object _propertyName;
        private Type _propertyType;

        internal DictionaryPropertyProxy(IDictionary dictionary, string propertyName)
        {
            _dictionary = dictionary;

            // If the given string property matches use it
            if (_dictionary.Contains(propertyName))
            {
                this._propertyName = propertyName;
            }
            else
            {
                // Try to find the key by comparing keys as strings
                foreach (var dictionaryKey in _dictionary.Keys)
                {
                    if (string.Equals(dictionaryKey.ToString(), propertyName, StringComparison.Ordinal))
                    {
                        this._propertyName = dictionaryKey;
                        break;
                    }
                }

                // If existing key was not found,
                // store the property name so it can be used for adding
                if (this._propertyName == null)
                {
                    this._propertyName = propertyName;
                }
            }
        }

        public object GetValue(object target)
        {
            var value = _dictionary[_propertyName];

            _propertyType = value?.GetType() ?? typeof(object);

            return value;
        }

        public void SetValue(object target, object convertedValue)
        {
            if (_propertyName != null)
            {
                if (convertedValue == null)
                {
                    _dictionary.Remove(_propertyName);
                }
                else
                {
                    _dictionary[_propertyName] = convertedValue;
                }
            } else if (convertedValue != null)
            {
                _dictionary.Add(_propertyName, convertedValue);
            }

            _propertyType = convertedValue?.GetType() ?? typeof(object);
        }

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType => _propertyType != null ? GetValue(null)?.GetType() : typeof(object);
    }
}
