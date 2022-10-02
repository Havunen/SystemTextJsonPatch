![SystemTextJsonPatchLogo](https://raw.githubusercontent.com/Havunen/SystemTextJsonPatch/main/logo.png)

[![CI status](https://img.shields.io/github/workflow/status/Havunen/SystemTextJsonPatch/CI?logo=GitHub)](https://github.com/Havunen/SystemTextJsonPatch/actions/workflows/ci.yml)
[![Nuget](https://img.shields.io/nuget/v/SystemTextJsonPatch?color=teal&logo=Nuget)](https://www.nuget.org/packages/SystemTextJsonPatch#readme-body-tab)


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

## Performance comparison

This test deserializes a JSON patch document of 5 operations and applies the changes to a new model.

See [SystemTextJsonPatch.Benchmark](https://github.com/Havunen/SystemTextJsonPatch/tree/main/SystemTextJsonPatch.Benchmark) for more details.

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.978/21H2)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=7.0.100-rc.1.22431.12
  [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  Job-MLBFMD : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2

WarmupCount=2

|              Method |       Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|-------------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
| SystemTextJsonPatch |   4.220 us | 0.0258 us | 0.0241 us | 0.1907 |      - |   3.21 KB |
|     MarvinJsonPatch | 781.650 us | 9.1887 us | 8.5951 us | 3.9063 | 1.9531 |  73.17 KB |
| AspNetCoreJsonPatch |  17.864 us | 0.1776 us | 0.1483 us | 1.6174 | 0.0305 |  26.81 KB |