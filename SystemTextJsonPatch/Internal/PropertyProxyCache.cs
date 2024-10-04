using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Internal.Proxies;

namespace SystemTextJsonPatch.Internal
{
	internal static class PropertyProxyCache
	{
		private static readonly ConcurrentDictionary<Type, PropertyInfo[]> CachedTypeProperties = new();
		private static readonly ConcurrentDictionary<(Type, string), PropertyProxy?> CachedPropertyProxies = new();

		internal static PropertyProxy? GetPropertyProxy(Type type, string propName)
		{
			var key = (type, propName);

			if (CachedPropertyProxies.TryGetValue(key, out var propertyProxy))
			{
				return propertyProxy;
			}

			if (!CachedTypeProperties.TryGetValue(type, out var properties))
			{
				properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				CachedTypeProperties[type] = properties;
			}

			propertyProxy = FindPropertyInfo(properties, propName);
			CachedPropertyProxies[key] = propertyProxy;

			return propertyProxy;
		}

		private static PropertyProxy? FindPropertyInfo(PropertyInfo[] properties, string propName)
		{
			// First check through all properties if property name matches JsonPropertyNameAttribute
			foreach (var propertyInfo in properties)
			{
				var jsonPropertyNameAttr = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
				if (jsonPropertyNameAttr != null && string.Equals(jsonPropertyNameAttr.Name, propName, StringComparison.OrdinalIgnoreCase))
                {
                    EnsureAccessToProperty(propertyInfo);
                    return new PropertyProxy(propertyInfo);
                }
            }

			// If it didn't find match by JsonPropertyName then use property name
			foreach (var propertyInfo in properties)
			{
				if (string.Equals(propertyInfo.Name, propName, StringComparison.OrdinalIgnoreCase))
                {
                    EnsureAccessToProperty(propertyInfo);
                    return new PropertyProxy(propertyInfo);
				}
			}

			return null;
		}

        private static void EnsureAccessToProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetCustomAttribute(typeof(DenyPatchAttribute), true) != null)
            {
                throw new JsonPatchAccessDeniedException(propertyInfo);
            }
        }
    }
}
