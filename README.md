![SystemTextJsonPatchLogo](https://raw.githubusercontent.com/Havunen/SystemTextJsonPatch/main/logo.png)

# SystemTextJsonPatch

SystemTextJson is JSON JSON Patch (JsonPatchDocument) RFC 6902 implementation for .NET using System.Text.Json

This library tries to ease the migration from Newtonsoft.Json to System.Text.Json by providing
similar API for HttpPatch requests as in [Microsoft.AspNetCore.JsonPatch](https://github.com/dotnet/aspnetcore/tree/main/src/Features/JsonPatch) and [Marvin.JsonPatch](https://github.com/KevinDockx/JsonPatch)

* Designed as easy replacement for Microsoft.AspNetCore.JsonPatch, etc.
* Supports .NET 6.0+