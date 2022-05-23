![SystemTextJsonPatchLogo](https://raw.githubusercontent.com/Havunen/SystemTextJsonPatch/main/logo.png)

# SystemTextJsonPatch

SystemTextJsonPatch is a JSON Patch (JsonPatchDocument) RFC 6902 implementation for .NET using System.Text.Json

This library tries to ease the migration from Newtonsoft.Json to System.Text.Json by providing
similar API for HttpPatch requests as in [Microsoft.AspNetCore.JsonPatch](https://github.com/dotnet/aspnetcore/tree/main/src/Features/JsonPatch) and [Marvin.JsonPatch](https://github.com/KevinDockx/JsonPatch)

* Designed as an easy replacement for Microsoft.AspNetCore.JsonPatch
* Supports .NET 6.0+


## Getting started

To enable JsonPatchDocument serialization support add SystemTextJsonPatch.Converters.JsonPatchDocumentConverterFactory to System.Text.Json serialization options converters.
This is typically done in startup.cs file when configuring Asp.Net core MVC settings

```cs

public void ConfigureServices(IServiceCollection services)
{
    services
        .AddControllers()
        .AddJsonOptions((options) =>
        {
            options.JsonSerializerOptions.Converters.Add(new SystemTextJsonPatch.Converters.JsonPatchDocumentConverterFactory());
        });
}
```

or when using System.Text.Json.JsonSerializer directly with custom settings.

```cs
    var jsonOptions = new JsonSerializerOptions()
    {
        Converters =
        {
            new SystemTextJsonPatch.Converters.JsonPatchDocumentConverterFactory()
        }
    };

    var json = System.Text.Json.JsonSerializer.JsonSerializer.Serialize(incomingOperations, jsonOptions);
```