using System;

namespace SystemTextJsonPatch
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DenyPatchAttribute : Attribute
    {
    }
}