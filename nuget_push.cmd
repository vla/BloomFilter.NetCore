set /p ver=<VERSION
set sourceUrl=-s https://www.nuget.org/api/v2/package

dotnet nuget push artifacts/BloomFilter.NetCore.%ver%.nupkg %sourceUrl%
dotnet nuget push artifacts/BloomFilter.Redis.NetCore.%ver%.nupkg %sourceUrl%
dotnet nuget push artifacts/BloomFilter.CSRedis.NetCore.%ver%.nupkg %sourceUrl%
dotnet nuget push artifacts/BloomFilter.FreeRedis.NetCore.%ver%.nupkg %sourceUrl%
dotnet nuget push artifacts/BloomFilter.EasyCaching.NetCore.%ver%.nupkg %sourceUrl%
