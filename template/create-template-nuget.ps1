Copy-Item -Recurse ..\src .
dotnet pack -c Release
Remove-Item -Recurse -Force .\src