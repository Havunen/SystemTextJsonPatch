dotnet build --configuration Release SystemTextJsonPatch/SystemTextJsonPatch.csproj /p:Configuration=Release
dotnet build --configuration Release SystemTextJsonPatch.AspNet/SystemTextJsonPatch.AspNet.csproj /p:Configuration=Release

dotnet pack --configuration Release SystemTextJsonPatch/SystemTextJsonPatch.csproj
dotnet pack --configuration Release SystemTextJsonPatch.AspNet/SystemTextJsonPatch.AspNet.csproj