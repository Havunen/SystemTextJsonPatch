using Microsoft.AspNetCore.Http.Json;
using SystemTextJsonPatch.Converters;

namespace Microsoft.Extensions.DependencyInjection;

public static class HttpJsonOptionsExtensions
{
    /// <summary>
    /// Adds <see cref="SystemTextJsonPatch.JsonPatchDocument"/> and serializer to System.Text.Json.
    /// </summary>
    public static JsonOptions UseJsonPatchDocumentConverters(this JsonOptions options)
    {
        options.SerializerOptions.Converters.Add(new JsonPatchDocumentConverterFactory());
        return options;
    }
}
