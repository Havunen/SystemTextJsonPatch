using System;

namespace SystemTextJsonPatch.Internal.Proxies
{
    public interface IPropertyProxy
    {
        object GetValue(object target);
        void SetValue(object target, object convertedValue);
        bool CanRead { get; }
        bool CanWrite { get; }
        Type PropertyType { get; }
    }
}
