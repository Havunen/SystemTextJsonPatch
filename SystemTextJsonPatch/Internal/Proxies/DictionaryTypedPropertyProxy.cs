using System;
using System.Collections.Generic;

namespace SystemTextJsonPatch.Internal.Proxies
{
	internal sealed class DictionaryTypedPropertyProxy<TKey, TValue> : IPropertyProxy
	{
		private readonly IDictionary<TKey, TValue?> _dictionary;
		private readonly TKey _propertyName;

		internal DictionaryTypedPropertyProxy(IDictionary<TKey, TValue?> dictionary, TKey propertyName)
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
				_dictionary[_propertyName] = (TValue?)convertedValue;
			}
			else
			{
				_dictionary.Add(_propertyName, (TValue?)convertedValue);
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
					return typeof(TValue);
				}

				return val.GetType();
			}
		}
	}
}
