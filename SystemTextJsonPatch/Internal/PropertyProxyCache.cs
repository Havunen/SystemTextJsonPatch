using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch.Exceptions;
using SystemTextJsonPatch.Internal.Proxies;

namespace SystemTextJsonPatch.Internal
{
	internal static class PropertyProxyCache
	{
		private static readonly ConcurrentDictionary<Type, PropertyInfo[]> CachedTypeProperties = new();
		// Naming policy has to be part of the key because it can change the target property
		private static readonly ConcurrentDictionary<(Type, string, JsonNamingPolicy?), PropertyProxy?> CachedPropertyProxies = new();

		internal static PropertyProxy? GetPropertyProxy(Type type, string propName, JsonNamingPolicy? namingPolicy, bool? propertyNameCaseInsensitive)
		{
			var key = (type, propName, namingPolicy);

			if (CachedPropertyProxies.TryGetValue(key, out var propertyProxy))
			{
				return propertyProxy;
			}

			if (!CachedTypeProperties.TryGetValue(type, out var properties))
			{
				properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				CachedTypeProperties[type] = properties;
			}

			propertyProxy = FindPropertyInfo(properties, propName, namingPolicy, propertyNameCaseInsensitive);
			CachedPropertyProxies[key] = propertyProxy;

			return propertyProxy;
		}

		private static PropertyProxy? FindPropertyInfo(PropertyInfo[] properties, string propName, JsonNamingPolicy? namingPolicy, bool? propertyNameCaseInsensitive)
		{
			// First check through all properties if property name matches JsonPropertyNameAttribute
			foreach (var propertyInfo in properties)
			{
				var jsonPropertyNameAttr = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
				if (jsonPropertyNameAttr != null && string.Equals(jsonPropertyNameAttr.Name, propName, StringComparison.Ordinal))
				{
					EnsureAccessToProperty(propertyInfo);
					return new PropertyProxy(propertyInfo);
				}
			}

			// If it didn't find match by JsonPropertyName then use property name
			foreach (var propertyInfo in properties)
			{
				var propertyName = namingPolicy != null ? namingPolicy.ConvertName(propertyInfo.Name) : propertyInfo.Name;

				var comparison = propertyNameCaseInsensitive == true ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

				if (string.Equals(propertyName, propName, comparison))
				{
					EnsureAccessToProperty(propertyInfo);
					return new PropertyProxy(propertyInfo);
				}
			}

			return null;
		}

		private static void EnsureAccessToProperty(PropertyInfo propertyInfo)
		{
			if (propertyInfo.GetCustomAttribute<DenyPatchAttribute>(true) != null)
			{
				throw new JsonPatchAccessDeniedException(propertyInfo);
			}
		}
	}
}
