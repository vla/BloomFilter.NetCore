# BloomFilter.NetCore

Library  Bloom filters in C#

## Packages & Status

Package  | NuGet         |
-------- | :------------ |
|**BloomFilter.NetCore**|[![NuGet package](https://buildstats.info/nuget/BloomFilter.NetCore)](https://www.nuget.org/packages/BloomFilter.NetCore)
|**BloomFilter.Redis.NetCore**|[![NuGet package](https://buildstats.info/nuget/BloomFilter.Redis.NetCore)](https://www.nuget.org/packages/BloomFilter.Redis.NetCore)
|**BloomFilter.CSRedis.NetCore**|[![NuGet package](https://buildstats.info/nuget/BloomFilter.CSRedis.NetCore)](https://www.nuget.org/packages/BloomFilter.CSRedis.NetCore)
|**BloomFilter.FreeRedis.NetCore**|[![NuGet package](https://buildstats.info/nuget/BloomFilter.FreeRedis.NetCore)](https://www.nuget.org/packages/BloomFilter.FreeRedis.NetCore)
|**BloomFilter.EasyCaching.NetCore**|[![NuGet package](https://buildstats.info/nuget/BloomFilter.EasyCaching.NetCore)](https://www.nuget.org/packages/BloomFilter.EasyCaching.NetCore)

## Usage

In Memory

```cs
    public class Demo
    {
        static IBloomFilter bf = FilterBuilder.Build(10000000, 0.01);

        public void Sample()
        {
            bf.Add("Value");
            Console.WriteLine(bf.Contains("Value"));
        }
    }

```

Configurations

```cs
var services = new ServiceCollection();
services.AddBloomFilter(setupAction =>
{
    setupAction.UseInMemory();
});

var provider = services.BuildServiceProvider();
var bf = provider.GetService<IBloomFilter>();
bf.Add("Value");
Console.WriteLine(bf.Contains("Value"));
```

Use Redis

```cs
    public class Demo
    {
        static IBloomFilter bf = FilterRedisBuilder.Build("localhost", "InstanceName", 5000000, 0.001);

        public void Sample()
        {
            bf.Add("Value");
            Console.WriteLine(bf.Contains("Value"));
        }
    }
```

StackExchange.Redis

```cs
var services = new ServiceCollection();
services.AddBloomFilter(setupAction =>
{
    setupAction.UseRedis(new FilterRedisOptions
    {
        Name = "Redis1",
        RedisKey = "BloomFilter1",
        Endpoints = new[] { "localhost" }.ToList()
    });
});

var provider = services.BuildServiceProvider();
var bf = provider.GetService<IBloomFilter>();
bf.Add("Value");
Console.WriteLine(bf.Contains("Value"));
```

CSRedisCore

```cs
var services = new ServiceCollection();
services.AddBloomFilter(setupAction =>
{
    setupAction.UseCSRedis(new FilterCSRedisOptions
    {
        Name = "Redis1",
        RedisKey = "CSRedis1",
        ConnectionStrings = new[] { "localhost" }.ToList()
    });
});

var provider = services.BuildServiceProvider();
var bf = provider.GetService<IBloomFilter>();
bf.Add("Value");
Console.WriteLine(bf.Contains("Value"));
```

FreeRedis

```cs
var services = new ServiceCollection();
services.AddBloomFilter(setupAction =>
{
    setupAction.UseFreeRedis(new FilterFreeRedisOptions
    {
        Name = "Redis1",
        RedisKey = "FreeRedis1",
        ConnectionStrings = new[] { "localhost" }.ToList()
    });
});

var provider = services.BuildServiceProvider();
var bf = provider.GetService<IBloomFilter>();
bf.Add("Value");
Console.WriteLine(bf.Contains("Value"));
```

EasyCaching

```cs
var services = new ServiceCollection();

services.AddEasyCaching(setupAction =>
{
    setupAction.UseCSRedis(configure =>
    {
        configure.DBConfig = new CSRedisDBOptions
        {
            ConnectionStrings = new System.Collections.Generic.List<string>
            {
                "127.0.0.1,defaultDatabase=0,poolsize=10"
            }
        };
    }, "BloomFilter1");

    setupAction.UseCSRedis(configure =>
    {
        configure.DBConfig = new CSRedisDBOptions
        {
            ConnectionStrings = new System.Collections.Generic.List<string>
            {
                "127.0.0.1,defaultDatabase=1,poolsize=10"
            }
        };
    }, "BloomFilter2");
});

services.AddBloomFilter(setupAction =>
{
    setupAction.UseEasyCachingRedis(new FilterEasyCachingRedisOptions
    {
        Name = "BF1",
        RedisKey = "EasyCaching1",
        ProviderName = "BloomFilter1"
    });

    //BloomFilter2
    setupAction.UseEasyCachingRedis(new FilterEasyCachingRedisOptions
    {
        Name = "BF2",
        RedisKey = "EasyCaching1"
    });
});

var provider = services.BuildServiceProvider();
var factory = provider.GetService<IBloomFilterFactory>();
var bf = provider.GetService<IBloomFilter>();
var bf1 = factory.Get("BF1");
bf.Add("Value");
Console.WriteLine(bf.Contains("Value"));
bf1.Add("Value");
Console.WriteLine(bf1.Contains("Value"));
```

## Benchmark

``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1555/22H2/2022Update/SunValley2)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2


```
| Method | DataSize |           Method |            Mean |        Error |       StdDev |             Min |             Max | Rank |   Gen0 | Allocated |
|------- |--------- |----------------- |----------------:|-------------:|-------------:|----------------:|----------------:|-----:|-------:|----------:|
|    **Add** |       **64** |      **LCGWithFNV1** |        **82.11 ns** |     **0.171 ns** |     **0.151 ns** |        **81.88 ns** |        **82.41 ns** |    **8** | **0.0033** |      **56 B** |
|    **Add** |       **64** |     **LCGWithFNV1a** |        **82.19 ns** |     **0.103 ns** |     **0.080 ns** |        **82.04 ns** |        **82.34 ns** |    **8** | **0.0033** |      **56 B** |
|    **Add** |       **64** |  **LCGModifiedFNV1** |        **84.18 ns** |     **0.149 ns** |     **0.139 ns** |        **84.04 ns** |        **84.46 ns** |    **9** | **0.0033** |      **56 B** |
|    **Add** |       **64** |      **RNGWithFNV1** |       **403.35 ns** |     **0.653 ns** |     **0.611 ns** |       **402.21 ns** |       **404.18 ns** |   **16** | **0.0215** |     **360 B** |
|    **Add** |       **64** |     **RNGWithFNV1a** |       **456.20 ns** |     **1.984 ns** |     **1.856 ns** |       **454.19 ns** |       **460.05 ns** |   **19** | **0.0215** |     **360 B** |
|    **Add** |       **64** |  **RNGModifiedFNV1** |       **443.46 ns** |     **1.070 ns** |     **1.001 ns** |       **442.26 ns** |       **445.77 ns** |   **18** | **0.0215** |     **360 B** |
|    **Add** |       **64** |            **CRC32** |       **124.93 ns** |     **0.634 ns** |     **0.593 ns** |       **124.02 ns** |       **125.63 ns** |   **10** | **0.0181** |     **304 B** |
|    **Add** |       **64** |          **Adler32** |       **130.63 ns** |     **0.724 ns** |     **0.677 ns** |       **129.34 ns** |       **131.72 ns** |   **11** | **0.0186** |     **312 B** |
|    **Add** |       **64** |          **Murmur3** |        **63.25 ns** |     **0.194 ns** |     **0.182 ns** |        **63.01 ns** |        **63.55 ns** |    **5** | **0.0033** |      **56 B** |
|    **Add** |       **64** |  **Murmur32BitsX86** |        **63.32 ns** |     **0.263 ns** |     **0.246 ns** |        **63.06 ns** |        **63.83 ns** |    **5** | **0.0033** |      **56 B** |
|    **Add** |       **64** | **Murmur128BitsX64** |        **70.82 ns** |     **0.212 ns** |     **0.199 ns** |        **70.58 ns** |        **71.20 ns** |    **7** | **0.0057** |      **96 B** |
|    **Add** |       **64** | **Murmur128BitsX86** |        **71.63 ns** |     **0.234 ns** |     **0.208 ns** |        **71.37 ns** |        **72.03 ns** |    **7** | **0.0057** |      **96 B** |
|    **Add** |       **64** |             **SHA1** |     **1,068.66 ns** |     **2.721 ns** |     **2.412 ns** |     **1,063.18 ns** |     **1,072.44 ns** |   **25** | **0.0248** |     **440 B** |
|    **Add** |       **64** |           **SHA256** |       **951.26 ns** |     **5.269 ns** |     **4.114 ns** |       **946.11 ns** |       **960.25 ns** |   **23** | **0.0277** |     **472 B** |
|    **Add** |       **64** |           **SHA384** |     **1,054.71 ns** |     **1.846 ns** |     **1.542 ns** |     **1,051.42 ns** |     **1,057.60 ns** |   **24** | **0.0248** |     **432 B** |
|    **Add** |       **64** |           **SHA512** |     **1,067.70 ns** |     **3.529 ns** |     **3.301 ns** |     **1,062.76 ns** |     **1,073.66 ns** |   **25** | **0.0286** |     **480 B** |
|    **Add** |       **64** |         **XXHash32** |        **65.96 ns** |     **0.309 ns** |     **0.289 ns** |        **65.65 ns** |        **66.45 ns** |    **6** | **0.0033** |      **56 B** |
|    **Add** |       **64** |         **XXHash64** |        **44.34 ns** |     **0.107 ns** |     **0.100 ns** |        **44.13 ns** |        **44.46 ns** |    **3** | **0.0033** |      **56 B** |
|    **Add** |       **64** |          **XXHash3** |        **27.71 ns** |     **0.160 ns** |     **0.150 ns** |        **27.42 ns** |        **27.89 ns** |    **1** | **0.0033** |      **56 B** |
|    **Add** |       **64** |        **XXHash128** |        **29.49 ns** |     **0.132 ns** |     **0.124 ns** |        **29.29 ns** |        **29.62 ns** |    **2** | **0.0033** |      **56 B** |
|    **Add** |     **1024** |      **LCGWithFNV1** |       **875.04 ns** |     **0.182 ns** |     **0.161 ns** |       **874.72 ns** |       **875.28 ns** |   **22** | **0.0029** |      **56 B** |
|    **Add** |     **1024** |     **LCGWithFNV1a** |       **875.61 ns** |     **0.215 ns** |     **0.201 ns** |       **875.29 ns** |       **876.02 ns** |   **22** | **0.0029** |      **56 B** |
|    **Add** |     **1024** |  **LCGModifiedFNV1** |       **877.43 ns** |     **0.161 ns** |     **0.143 ns** |       **877.21 ns** |       **877.65 ns** |   **22** | **0.0029** |      **56 B** |
|    **Add** |     **1024** |      **RNGWithFNV1** |     **1,181.36 ns** |     **1.622 ns** |     **1.517 ns** |     **1,178.88 ns** |     **1,183.97 ns** |   **26** | **0.0210** |     **360 B** |
|    **Add** |     **1024** |     **RNGWithFNV1a** |     **1,193.84 ns** |     **1.548 ns** |     **1.293 ns** |     **1,191.54 ns** |     **1,196.66 ns** |   **26** | **0.0210** |     **360 B** |
|    **Add** |     **1024** |  **RNGModifiedFNV1** |     **1,253.63 ns** |     **1.613 ns** |     **1.509 ns** |     **1,251.14 ns** |     **1,256.17 ns** |   **27** | **0.0210** |     **360 B** |
|    **Add** |     **1024** |            **CRC32** |       **485.83 ns** |     **0.752 ns** |     **0.704 ns** |       **484.84 ns** |       **487.44 ns** |   **21** | **0.0181** |     **304 B** |
|    **Add** |     **1024** |          **Adler32** |       **298.43 ns** |     **0.494 ns** |     **0.413 ns** |       **297.78 ns** |       **299.33 ns** |   **14** | **0.0186** |     **312 B** |
|    **Add** |     **1024** |          **Murmur3** |       **475.53 ns** |     **1.751 ns** |     **1.637 ns** |       **474.09 ns** |       **478.18 ns** |   **20** | **0.0029** |      **56 B** |
|    **Add** |     **1024** |  **Murmur32BitsX86** |       **477.08 ns** |     **1.750 ns** |     **1.637 ns** |       **473.92 ns** |       **478.53 ns** |   **20** | **0.0029** |      **56 B** |
|    **Add** |     **1024** | **Murmur128BitsX64** |       **233.92 ns** |     **0.833 ns** |     **0.779 ns** |       **233.00 ns** |       **235.30 ns** |   **13** | **0.0057** |      **96 B** |
|    **Add** |     **1024** | **Murmur128BitsX86** |       **332.03 ns** |     **1.255 ns** |     **1.174 ns** |       **330.29 ns** |       **333.50 ns** |   **15** | **0.0057** |      **96 B** |
|    **Add** |     **1024** |             **SHA1** |     **4,517.51 ns** |     **6.470 ns** |     **5.736 ns** |     **4,511.70 ns** |     **4,529.08 ns** |   **31** | **0.0305** |     **520 B** |
|    **Add** |     **1024** |           **SHA256** |     **1,805.80 ns** |     **1.300 ns** |     **1.086 ns** |     **1,804.18 ns** |     **1,807.48 ns** |   **28** | **0.0267** |     **472 B** |
|    **Add** |     **1024** |           **SHA384** |     **2,230.52 ns** |     **3.855 ns** |     **3.417 ns** |     **2,225.81 ns** |     **2,238.27 ns** |   **29** | **0.0229** |     **432 B** |
|    **Add** |     **1024** |           **SHA512** |     **2,289.69 ns** |     **2.403 ns** |     **2.006 ns** |     **2,286.48 ns** |     **2,292.14 ns** |   **30** | **0.0267** |     **480 B** |
|    **Add** |     **1024** |         **XXHash32** |       **419.22 ns** |     **0.970 ns** |     **0.810 ns** |       **418.29 ns** |       **421.21 ns** |   **17** | **0.0033** |      **56 B** |
|    **Add** |     **1024** |         **XXHash64** |       **136.76 ns** |     **0.486 ns** |     **0.455 ns** |       **136.15 ns** |       **137.56 ns** |   **12** | **0.0033** |      **56 B** |
|    **Add** |     **1024** |          **XXHash3** |        **62.34 ns** |     **0.283 ns** |     **0.265 ns** |        **62.05 ns** |        **62.87 ns** |    **4** | **0.0033** |      **56 B** |
|    **Add** |     **1024** |        **XXHash128** |        **66.26 ns** |     **0.335 ns** |     **0.313 ns** |        **65.75 ns** |        **66.66 ns** |    **6** | **0.0033** |      **56 B** |
|    **Add** |  **1048576** |      **LCGWithFNV1** |   **865,640.00 ns** |   **106.264 ns** |    **82.964 ns** |   **865,422.75 ns** |   **865,728.03 ns** |   **40** |      **-** |      **57 B** |
|    **Add** |  **1048576** |     **LCGWithFNV1a** |   **865,629.16 ns** |   **150.949 ns** |   **141.197 ns** |   **865,402.73 ns** |   **865,909.38 ns** |   **40** |      **-** |      **57 B** |
|    **Add** |  **1048576** |  **LCGModifiedFNV1** |   **865,679.35 ns** |   **116.519 ns** |    **97.299 ns** |   **865,504.69 ns** |   **865,814.06 ns** |   **40** |      **-** |      **57 B** |
|    **Add** |  **1048576** |      **RNGWithFNV1** |   **866,702.12 ns** |   **938.153 ns** |   **877.549 ns** |   **865,962.89 ns** |   **868,312.89 ns** |   **40** |      **-** |     **361 B** |
|    **Add** |  **1048576** |     **RNGWithFNV1a** |   **866,101.68 ns** |   **115.846 ns** |    **96.737 ns** |   **865,858.69 ns** |   **866,232.03 ns** |   **40** |      **-** |     **361 B** |
|    **Add** |  **1048576** |  **RNGModifiedFNV1** |   **866,209.05 ns** |   **243.284 ns** |   **189.940 ns** |   **866,005.76 ns** |   **866,598.14 ns** |   **40** |      **-** |     **361 B** |
|    **Add** |  **1048576** |            **CRC32** |   **435,843.79 ns** |   **293.389 ns** |   **274.436 ns** |   **435,535.45 ns** |   **436,306.15 ns** |   **38** |      **-** |     **336 B** |
|    **Add** |  **1048576** |          **Adler32** |   **183,721.32 ns** |   **329.215 ns** |   **307.948 ns** |   **183,425.59 ns** |   **184,296.09 ns** |   **35** |      **-** |     **312 B** |
|    **Add** |  **1048576** |          **Murmur3** |   **442,601.72 ns** |   **596.958 ns** |   **558.395 ns** |   **441,892.48 ns** |   **443,775.34 ns** |   **39** |      **-** |      **56 B** |
|    **Add** |  **1048576** |  **Murmur32BitsX86** |   **443,845.78 ns** | **1,809.064 ns** | **1,692.200 ns** |   **442,030.32 ns** |   **445,936.62 ns** |   **39** |      **-** |      **56 B** |
|    **Add** |  **1048576** | **Murmur128BitsX64** |   **166,783.05 ns** |   **527.239 ns** |   **493.180 ns** |   **166,289.79 ns** |   **167,638.33 ns** |   **34** |      **-** |      **96 B** |
|    **Add** |  **1048576** | **Murmur128BitsX86** |   **276,355.14 ns** |   **744.545 ns** |   **696.448 ns** |   **275,641.11 ns** |   **277,720.56 ns** |   **36** |      **-** |      **96 B** |
|    **Add** |  **1048576** |             **SHA1** | **4,512,589.00 ns** | **3,913.460 ns** | **3,267.916 ns** | **4,508,383.59 ns** | **4,520,710.16 ns** |   **43** |      **-** |     **605 B** |
|    **Add** |  **1048576** |           **SHA256** |   **875,367.38 ns** |    **82.713 ns** |    **73.322 ns** |   **875,209.77 ns** |   **875,472.36 ns** |   **41** |      **-** |     **473 B** |
|    **Add** |  **1048576** |           **SHA384** | **1,309,854.54 ns** | **2,593.708 ns** | **2,426.156 ns** | **1,307,542.77 ns** | **1,313,727.15 ns** |   **42** |      **-** |     **433 B** |
|    **Add** |  **1048576** |           **SHA512** | **1,309,829.94 ns** | **3,233.664 ns** | **2,866.559 ns** | **1,306,866.60 ns** | **1,316,294.53 ns** |   **42** |      **-** |     **481 B** |
|    **Add** |  **1048576** |         **XXHash32** |   **383,596.58 ns** |   **866.871 ns** |   **723.877 ns** |   **382,977.25 ns** |   **385,621.73 ns** |   **37** |      **-** |      **56 B** |
|    **Add** |  **1048576** |         **XXHash64** |   **101,663.14 ns** |   **261.367 ns** |   **244.483 ns** |   **101,109.01 ns** |   **102,027.89 ns** |   **33** |      **-** |      **56 B** |
|    **Add** |  **1048576** |          **XXHash3** |    **30,111.96 ns** |    **67.418 ns** |    **59.764 ns** |    **30,037.65 ns** |    **30,186.83 ns** |   **32** |      **-** |      **56 B** |
|    **Add** |  **1048576** |        **XXHash128** |    **30,310.57 ns** |    **72.530 ns** |    **67.845 ns** |    **30,226.71 ns** |    **30,434.61 ns** |   **32** |      **-** |      **56 B** |

## Hash ErrRate

Count:100000 Capacity:958506 Hashes:7 ExpectedElements:100000 ErrorRate:0.01

``` ini
LCGWithFNV1
 Speed:69.7668ms ErrRate:93.967% ErrTotal:93967 Final ErrRate:100.000%
LCGWithFNV1a
 Speed:101.6993ms ErrRate:93.906% ErrTotal:93906 Final ErrRate:100.000%
LCGModifiedFNV1
 Speed:58.1464ms ErrRate:93.942% ErrTotal:93942 Final ErrRate:100.000%
RNGWithFNV1
 Speed:209.1069ms ErrRate:0.171% ErrTotal:171 Final ErrRate:0.957%
RNGWithFNV1a
 Speed:210.2195ms ErrRate:0.174% ErrTotal:174 Final ErrRate:0.989%
RNGModifiedFNV1
 Speed:210.888ms ErrRate:0.177% ErrTotal:177 Final ErrRate:1.033%
CRC32
 Speed:75.1132ms ErrRate:0.182% ErrTotal:182 Final ErrRate:0.997%
Adler32
 Speed:53.8629ms ErrRate:10.246% ErrTotal:10246 Final ErrRate:23.316%
Murmur3
 Speed:40.8874ms ErrRate:0.156% ErrTotal:156 Final ErrRate:1.054%
Murmur32BitsX86
 Speed:40.9386ms ErrRate:0.156% ErrTotal:156 Final ErrRate:1.054%
Murmur128BitsX64
 Speed:31.3446ms ErrRate:0.168% ErrTotal:168 Final ErrRate:0.982%
Murmur128BitsX86
 Speed:36.43ms ErrRate:0.150% ErrTotal:150 Final ErrRate:0.993%
SHA1
 Speed:293.0176ms ErrRate:0.184% ErrTotal:184 Final ErrRate:1.013%
SHA256
 Speed:200.2562ms ErrRate:0.183% ErrTotal:183 Final ErrRate:0.972%
SHA384
 Speed:299.5489ms ErrRate:0.171% ErrTotal:171 Final ErrRate:0.947%
SHA512
 Speed:305.8543ms ErrRate:0.159% ErrTotal:159 Final ErrRate:1.055%
XXHash32
 Speed:37.8243ms ErrRate:0.146% ErrTotal:146 Final ErrRate:1.022%
XXHash64
 Speed:21.5273ms ErrRate:0.200% ErrTotal:200 Final ErrRate:0.989%
XXHash3
 Speed:17.5158ms ErrRate:0.149% ErrTotal:149 Final ErrRate:0.964%
XXHash128
 Speed:19.129ms ErrRate:0.183% ErrTotal:183 Final ErrRate:1.002%
```
