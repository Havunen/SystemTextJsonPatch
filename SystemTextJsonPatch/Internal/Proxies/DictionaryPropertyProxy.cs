using System;
using System.Collections;

namespace SystemTextJsonPatch.Internal.Proxies
{
	internal sealed class DictionaryPropertyProxy : IPropertyProxy
	{
		private readonly IDictionary _dictionary;
		private readonly object _propertyName;

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

		public object? GetValue(object target)
		{
			var value = _dictionary[_propertyName];

			return value;
		}

		public void SetValue(object target, object? convertedValue)
		{
			if (_dictionary.Contains(_propertyName))
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
				var val = _dictionary[_propertyName];
				if (val == null)
				{
					return typeof(object);
				}

				return val.GetType();
			}
		}
	}
}
