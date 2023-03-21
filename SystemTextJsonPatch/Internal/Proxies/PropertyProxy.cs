using System;
using System.Reflection;

namespace SystemTextJsonPatch.Internal.Proxies
{
	internal sealed class PropertyProxy : IPropertyProxy
	{
		private readonly PropertyInfo _propertyInfo;
		internal PropertyProxy(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		public object? GetValue(object target)
		{
			return _propertyInfo.GetValue(target);
		}

		public void SetValue(object target, object? convertedValue)
		{
			_propertyInfo.SetValue(target, convertedValue);
		}

		public void RemoveValue(object target)
		{
			_propertyInfo.SetValue(target, null);
		}

		public bool CanRead => _propertyInfo.CanRead;
		public bool CanWrite => _propertyInfo.CanWrite;
		public Type PropertyType => _propertyInfo.PropertyType;
	}
}
