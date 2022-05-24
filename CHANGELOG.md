## 0.0.4
- Fixed an issue where null value was not accepted for patch operation

## 0.0.3
- Fixes an issue where custom json converter JsonException messages were overriden in SystemTextJsonPatch
- Added CI build. Thanks to https://github.com/ilmax https://github.com/Havunen/SystemTextJsonPatch/pull/3


## 0.0.2

- Combined SystemTextJson.AspNet and SystemTextJson packages
- Fixes an issue where PATCH error message value did not match the actual value used. (https://github.com/dotnet/aspnetcore/issues/38872)