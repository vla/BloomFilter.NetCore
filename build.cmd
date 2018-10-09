set artifacts=%~dp0artifacts

if exist %artifacts%  rd /q /s %artifacts%

call dotnet restore src/BloomFilter
call dotnet restore src/BloomFilter.Redis


call dotnet pack src/BloomFilter -c release -o %artifacts%
call dotnet pack src/BloomFilter.Redis -c release -o %artifacts%
