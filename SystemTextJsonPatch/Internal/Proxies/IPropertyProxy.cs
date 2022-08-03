using System;

namespace SystemTextJsonPatch.Internal.Proxies
{
    public interface IPropertyProxy
    {
        object? GetValue(object target);
        void SetValue(object target, object? convertedValue);
        void RemoveValue(object target);
        bool CanRead { get; }
        bool CanWrite { get; }
        Type PropertyType { get; }
    }
}
