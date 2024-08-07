## 3.2.1
- Fixes an issue where null and empty string test operation of JsonNode was incorrectly handled https://github.com/Havunen/SystemTextJsonPatch/issues/31

## 3.2.0
- Adds support for dictionary complex types https://github.com/Havunen/SystemTextJsonPatch/pull/29
- Internal dependencies updated

## 3.1.0
- Added .NET 8 target framework

## 3.0.1
- All invalid patch operations now correctly throw JsonPatchTestOperationException and malformed patch documents throw JsonPatchException


## 3.0.0
- Library runtime performance is improved by using throw helpers for throwing exceptions https://github.com/Havunen/SystemTextJsonPatch/issues/21
- **Possibly breaking change**: `JsonPatchTestOperationException` type is now thrown for `test` -operation failures https://github.com/Havunen/SystemTextJsonPatch/issues/22


## 2.0.2
- Fixes an issue where dynamic JsonPatchDocument did not follow `JsonNamingPolicy` option for property names. https://github.com/Havunen/SystemTextJsonPatch/issues/19
- Small performance improvements https://github.com/Havunen/SystemTextJsonPatch/commit/da51501d668cddcda000b08e06d28cff75e7a0ce

## 2.0.1
- Fixes IndexOutOfRangeException in parse path routine

## 2.0.0
- JsonPatchDocument types now adds required converters using JsonConverterAttribute and converters no longer need to be set manually to JsonSerializerOptions https://github.com/Havunen/SystemTextJsonPatch/issues/18
- netstandard2.0 target now depends on System.Text.Json v7
- Small performance optimizations

## 1.2.0
- Better support for patching arrays by index https://github.com/Havunen/SystemTextJsonPatch/issues/15

## 1.1.0
- Adds .NET 7 to target frameworks
- Updated internal Nuget dependencies
- Fixed readme badges

## 1.0.0
- Adds netstandard2.0 target framework
- Fixes an issue where removing from the end of JsonNode array did not work
- Small performance improvements

## 0.0.9
- Fixes an issue where copy operation ignored System.Text.Json options. Github https://github.com/Havunen/SystemTextJsonPatch/issues/13

## 0.0.8
- Code cleaning
- Fixes an issue where copying null string value to another dynamic object deleted the original property instead of setting it null
- Fixes an issue where operation value was serialized even when not needed
- Parsing logic now follows System.Text.Json.JsonSerializerOptions.PropertyNameCaseInsensitive option https://github.com/Havunen/SystemTextJsonPatch/issues/10
- Added MIT lience to Github

## 0.0.7
- Performance improvements (https://github.com/Havunen/SystemTextJsonPatch/commit/a7b7c66a920ed0c258dec987d92c4edb0f6534cb https://github.com/Havunen/SystemTextJsonPatch/commit/afd5fbcceee6c1b6be7ee914d258a009f29eb437 https://github.com/Havunen/SystemTextJsonPatch/commit/54a0ab815c88536c89ee976358d7b27c356aed49 ) Thanks to [campersau](https://github.com/campersau)

## 0.0.6
- Changed property type cache to be thread safe https://github.com/Havunen/SystemTextJsonPatch/commit/7e53a7d92f9cb9a3d4cd5ac7dcea915eeb825a82
- Fixes an issue comparing decimals when applying patch document changes https://github.com/Havunen/SystemTextJsonPatch/issues/4

## 0.0.5
- Improved performance

## 0.0.4
- Fixed an issue where null value was not accepted for patch operation

## 0.0.3
- Fixes an issue where custom json converter JsonException messages were overriden in SystemTextJsonPatch
- Added CI build. Thanks to https://github.com/ilmax https://github.com/Havunen/SystemTextJsonPatch/pull/3


## 0.0.2

- Combined SystemTextJson.AspNet and SystemTextJson packages
- Fixes an issue where PATCH error message value did not match the actual value used. (https://github.com/dotnet/aspnetcore/issues/38872)