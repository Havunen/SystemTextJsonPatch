using System;
using System.Dynamic;

namespace SystemTextJsonPatch.Internal.Proxies
{
    internal sealed class DynamicObjectPropertyProxy : IPropertyProxy
    {
        private readonly DynamicObject _dynamicObject;
        private readonly string _propertyName;

        internal DynamicObjectPropertyProxy(DynamicObject dynamicObject, string propertyName)
        {
            _dynamicObject = dynamicObject;
            _propertyName = propertyName;
        }

        public object? GetValue(object target)
        {
            _dynamicObject.TryGetMember(new DynamicGetMemberBinder(_propertyName, true), out var result);

            return result;
        }

        public void SetValue(object target, object? convertedValue)
        {
            _dynamicObject.TrySetMember(new DynamicSetMemberBinder(_propertyName, true), convertedValue);
        }

        public void RemoveValue(object target)
        {
            // C# and VB does not support deleting members from dynamic object
            // even tho TryDeleteMember method exists:
            // https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.dynamicobject.trydeletemember?view=net-6.0
            this.SetValue(target, null);
        }

        public bool CanRead => true;
        public bool CanWrite => true;
        public Type PropertyType
        {
            get
            {
                _dynamicObject.TryGetMember(new DynamicGetMemberBinder(_propertyName, true), out var result);

                if (result != null)
                {
                    return result.GetType();
                }

                return typeof(object);
            }
        }
    }
}
