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
        public JsonPatchAccessDeniedException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public JsonPatchAccessDeniedException(PropertyInfo propertyInfo) : this($"Patch is not allowed to access the property {propertyInfo?.Name ?? "N/A"} on the type {propertyInfo?.DeclaringType?.Name ?? "N/A"}", null)
        {
            
        }
    }
}

