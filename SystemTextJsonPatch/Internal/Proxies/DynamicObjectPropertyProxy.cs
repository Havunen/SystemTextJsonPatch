using System;
using System.Dynamic;

namespace SystemTextJsonPatch.Internal.Proxies
{
    internal sealed class DynamicObjectPropertyProxy : IPropertyProxy
    {
        private readonly DynamicObject _dynamicObject;
        private readonly string _propertyName;
        private Type _propertyType;

        internal DynamicObjectPropertyProxy(DynamicObject dynamicObject, string propertyName)
        {
            _dynamicObject = dynamicObject;
            _propertyName = propertyName;
        }

        public object GetValue(object target)
        {
            _dynamicObject.TryGetMember(new DynamicGetMemberBinder(_propertyName, true), out var result);

            _propertyType = result?.GetType() ?? typeof(object);

            return result;
        }

        public void SetValue(object target, object convertedValue)
        {
            _dynamicObject.TrySetMember(new DynamicSetMemberBinder(_propertyName, true), convertedValue);
            _propertyType = convertedValue?.GetType() ?? typeof(object);
        }

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType => _propertyType != null ? GetValue(null)?.GetType() : typeof(object);
    }
}
