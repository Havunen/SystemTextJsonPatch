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
var expensePatchJson = System.Text.Json.JsonSerializer.Serialize(expensePatch);
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

## Deny access to properties
If you need to stop JsonPatch from reading or writing to some properties,
then you can decorate them with `[DenyPatch]`, if a patch occurs that happens to access the property then a `JsonPatchAccessDeniedException` is thrown.


## Migration from v1

JsonPatchDocumentConverterFactory no longer needs to be set to JsonSerializerOptions.
Instead JsonPatchDocument types now use JsonConvertAttribute to use the correct converter.

## Performance comparison

This test deserializes a JSON patch document of 8 operations and applies the changes to a new model.

See [SystemTextJsonPatch.Benchmark](https://github.com/Havunen/SystemTextJsonPatch/tree/main/SystemTextJsonPatch.Benchmark) for more details.

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.400-preview.0.24324.5
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  Job-FECQKB : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2

WarmupCount=2

| Method              | Mean       | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|-------------------- |-----------:|----------:|----------:|-------:|-------:|----------:|
| SystemTextJsonPatch |   5.253 us | 0.0315 us | 0.0451 us | 0.3357 |      - |   5.52 KB |
| MarvinJsonPatch     | 830.553 us | 9.1462 us | 7.6375 us | 5.8594 | 3.9063 | 104.82 KB |
| AspNetCoreJsonPatch |  18.078 us | 0.1070 us | 0.0948 us | 2.9297 | 0.0610 |  48.35 KB |