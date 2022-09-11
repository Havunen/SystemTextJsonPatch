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