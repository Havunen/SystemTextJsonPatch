using Microsoft.AspNetCore.Mvc;
using SystemTextJsonPatch.Converters;

namespace Microsoft.Extensions.DependencyInjection;

public static class JsonOptionsExtensions
{
    /// <summary>
    /// Adds <see cref="SystemTextJsonPatch.JsonPatchDocument"/> and serializer to System.Text.Json.
    /// </summary>
    public static JsonOptions UseJsonPatchDocumentConverters(this JsonOptions options)
    {
        options.JsonSerializerOptions.Converters.Add(new JsonPatchDocumentConverterFactory());
        return options;
    }
}