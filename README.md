![SystemTextJsonPatchLogo](https://raw.githubusercontent.com/Havunen/SystemTextJsonPatch/main/logo.png)

[![CI status](https://img.shields.io/github/actions/workflow/status/Havunen/SystemTextJsonPatch/ci.yml?branch=main&logo=GitHub)](https://github.com/Havunen/SystemTextJsonPatch/actions/workflows/ci.yml)
[![Nuget](https://img.shields.io/nuget/v/SystemTextJsonPatch?color=teal&logo=Nuget)](https://www.nuget.org/packages/SystemTextJsonPatch#readme-body-tab)


# System Text Json Patch

SystemTextJsonPatch is a JSON Patch (JsonPatchDocument) RFC 6902 implementation for .NET using System.Text.Json

This library tries to ease the migration from Newtonsoft.Json to System.Text.Json by providing
similar API for HttpPatch requests as in [Microsoft.AspNetCore.JsonPatch](https://github.com/dotnet/aspnetcore/tree/main/src/Features/JsonPatch) and [Marvin.JsonPatch](https://github.com/KevinDockx/JsonPatch)

* Designed as an easy replacement for Microsoft.AspNetCore.JsonPatch
* Supports .NET 6+ & netstandard2.0


## Getting started

Build a patch document on the client.
You can use the operations as described in the IETF document: Add, Remove, Replace, Copy, Move and Test.

```cs
JsonPatchDocument<DTO.Expense> expensePatch = new JsonPatchDocument<DTO.Expense>();
expensePatch.Replace(e => e.Description, expense.Description);

// serialize it to JSON
var expensePatchJson = JsonConvert.SerializeObject(expensePatch);
```


On your API, in the patch method (accept document as parameter & use ApplyTo method)

```cs
[Route("api/expenses/{id}")]
[HttpPatch]
public IHttpActionResult Patch(
    int id,
    [FromBody] JsonPatchDocument<DTO.Expense> expensePatchDocument
)
{
      // get the expense from the repository
      var expense = _repository.GetExpense(id);

      // apply the patch document 
      expensePatchDocument.ApplyTo(expense);

      // changes have been applied to expense object
}
```

## Migration from v1

JsonPatchDocumentConverterFactory no longer needs to be set to JsonSerializerOptions.
Instead JsonPatchDocument types now use JsonConvertAttribute to use the correct converter.

## Performance comparison

This test deserializes a JSON patch document of 8 operations and applies the changes to a new model.

See [SystemTextJsonPatch.Benchmark](https://github.com/Havunen/SystemTextJsonPatch/tree/main/SystemTextJsonPatch.Benchmark) for more details.

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22000.978/21H2)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=7.0.100-rc.1.22431.12
  [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
  Job-URORCR : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2

WarmupCount=2

|              Method |       Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|-------------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
| SystemTextJsonPatch |   7.233 us | 0.0381 us | 0.0356 us | 0.3738 |      - |   6.16 KB |
|     MarvinJsonPatch | 979.525 us | 9.9310 us | 8.8036 us | 5.8594 | 3.9063 |  98.13 KB |
| AspNetCoreJsonPatch |  26.645 us | 0.2023 us | 0.1892 us | 2.5940 | 0.0610 |  42.49 KB |
