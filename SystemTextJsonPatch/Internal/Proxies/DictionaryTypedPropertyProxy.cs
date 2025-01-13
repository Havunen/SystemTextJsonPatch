using System;
using System.Collections.Generic;
using SystemTextJsonPatch.Exceptions;

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
			try
			{
				var value = _dictionary[_propertyName];
				return value;
			}
			catch (KeyNotFoundException)
			{
				throw new JsonPatchException(Resources.FormatTargetLocationAtPathSegmentNotFound(_propertyName), null);
			}
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

		public Type PropertyType => typeof(TValue);
	}
}
