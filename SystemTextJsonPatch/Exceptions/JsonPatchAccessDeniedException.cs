using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SystemTextJsonPatch.Exceptions
{

#pragma warning disable CA1032 // Implement standard exception constructors
    [Serializable]
    public class JsonPatchAccessDeniedException : JsonPatchException
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        public string Type { get; } = "N/A";
        public string Property { get; } = "N/A";
        public JsonPatchAccessDeniedException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public JsonPatchAccessDeniedException(string type, string property) : this($"Patch is not allowed to access the property {property} on the type {type}", (Exception?)null)
        {
            this.Type = type;
            this.Property = property;
        }

        public JsonPatchAccessDeniedException(PropertyInfo propertyInfo) : this(propertyInfo?.DeclaringType?.Name ?? "N/A", propertyInfo?.Name ?? "N/A")
        {
            
        }
    }
}

