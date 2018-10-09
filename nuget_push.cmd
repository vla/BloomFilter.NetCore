set /p ver=<VERSION
set sourceUrl=-s https://www.nuget.org/api/v2/package

dotnet nuget push artifacts/BloomFilter.NetCore.%ver%.nupkg %sourceUrl%
dotnet nuget push artifacts/BloomFilter.Redis.NetCore.%ver%.nupkg %sourceUrl%
