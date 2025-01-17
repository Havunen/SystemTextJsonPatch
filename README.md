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
List<Operation<DTO.Expense>> operations = [];
JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
JsonPatchDocument<DTO.Expense> expensePatch = new JsonPatchDocument<DTO.Expense>(operations,  jsonOptions);
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

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2605)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.200-preview.0.24575.35
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-ZCWSQV : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

WarmupCount=2

| Method              | Mean       | Error      | StdDev     | Gen0   | Gen1   | Allocated |
|-------------------- |-----------:|-----------:|-----------:|-------:|-------:|----------:|
| SystemTextJsonPatch |   3.909 us |  0.0616 us |  0.0576 us | 0.2747 |      - |   4.63 KB |
| MarvinJsonPatch     | 741.280 us | 14.2699 us | 11.9160 us | 3.9063 | 1.9531 |  95.09 KB |
| AspNetCoreJsonPatch |  15.268 us |  0.2227 us |  0.2084 us | 2.6245 | 0.0916 |  43.07 KB |