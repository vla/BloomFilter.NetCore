# BloomFilter.NetCore

[![Build Status](https://travis-ci.org/vla/BloomFilter.NetCore.svg?branch=master)](https://travis-ci.org/vla/BloomFilter.NetCore)

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

| Method | DataSize |               Method |        Mean |     Error |    StdDev |         Min |         Max | Rank |   Gen0 | Allocated |
|------- |--------- |--------------------- |------------:|----------:|----------:|------------:|------------:|-----:|-------:|----------:|
|    **Add** |       **64** |          **LCGWithFNV1** |    **70.84 ns** |  **0.108 ns** |  **0.096 ns** |    **70.71 ns** |    **71.04 ns** |    **3** | **0.0033** |      **56 B** |
|    **Add** |       **64** |         **LCGWithFNV1a** |    **71.30 ns** |  **0.101 ns** |  **0.095 ns** |    **71.13 ns** |    **71.43 ns** |    **3** | **0.0033** |      **56 B** |
|    **Add** |       **64** |      **LCGModifiedFNV1** |    **82.04 ns** |  **0.081 ns** |  **0.072 ns** |    **81.96 ns** |    **82.19 ns** |    **5** | **0.0033** |      **56 B** |
|    **Add** |       **64** |          **RNGWithFNV1** |   **334.09 ns** |  **1.151 ns** |  **0.961 ns** |   **332.86 ns** |   **335.50 ns** |    **9** | **0.0215** |     **360 B** |
|    **Add** |       **64** |         **RNGWithFNV1a** |   **333.15 ns** |  **0.633 ns** |  **0.592 ns** |   **332.28 ns** |   **334.07 ns** |    **9** | **0.0215** |     **360 B** |
|    **Add** |       **64** |      **RNGModifiedFNV1** |   **327.79 ns** |  **0.882 ns** |  **0.782 ns** |   **326.52 ns** |   **329.31 ns** |    **8** | **0.0215** |     **360 B** |
|    **Add** |       **64** |                **CRC32** |   **881.59 ns** | **14.044 ns** | **13.137 ns** |   **843.00 ns** |   **894.94 ns** |   **11** | **0.0057** |      **96 B** |
|    **Add** |       **64** |               **CRC32u** | **1,033.64 ns** |  **7.206 ns** |  **6.741 ns** | **1,011.03 ns** | **1,039.04 ns** |   **15** | **0.0057** |      **96 B** |
|    **Add** |       **64** |              **Adler32** |   **360.03 ns** |  **7.200 ns** | **12.029 ns** |   **346.10 ns** |   **376.29 ns** |   **10** | **0.0057** |      **96 B** |
|    **Add** |       **64** |              **Murmur2** |   **307.63 ns** |  **0.296 ns** |  **0.262 ns** |   **306.99 ns** |   **308.04 ns** |    **7** | **0.0086** |     **144 B** |
|    **Add** |       **64** |              **Murmur3** |   **205.51 ns** |  **0.335 ns** |  **0.280 ns** |   **205.15 ns** |   **206.10 ns** |    **6** | **0.0033** |      **56 B** |
|    **Add** |       **64** | **Murmu(...)acher [25]** |    **75.97 ns** |  **0.401 ns** |  **0.375 ns** |    **75.33 ns** |    **76.46 ns** |    **4** | **0.0033** |      **56 B** |
|    **Add** |       **64** |                 **SHA1** | **1,502.79 ns** |  **7.317 ns** |  **6.486 ns** | **1,489.20 ns** | **1,513.20 ns** |   **16** | **0.0458** |     **784 B** |
|    **Add** |       **64** |               **SHA256** |   **925.28 ns** |  **9.802 ns** |  **8.689 ns** |   **912.96 ns** |   **938.76 ns** |   **12** | **0.0391** |     **664 B** |
|    **Add** |       **64** |               **SHA384** |   **972.51 ns** |  **8.110 ns** |  **7.586 ns** |   **961.46 ns** |   **984.36 ns** |   **14** | **0.0305** |     **528 B** |
|    **Add** |       **64** |               **SHA512** |   **944.82 ns** |  **3.865 ns** |  **3.615 ns** |   **939.07 ns** |   **949.79 ns** |   **13** | **0.0353** |     **592 B** |
|    **Add** |       **64** |             **XXHash32** |    **39.12 ns** |  **0.044 ns** |  **0.039 ns** |    **39.04 ns** |    **39.17 ns** |    **2** | **0.0033** |      **56 B** |
|    **Add** |       **64** |             **XXHash64** |    **32.19 ns** |  **0.140 ns** |  **0.124 ns** |    **32.03 ns** |    **32.45 ns** |    **1** | **0.0033** |      **56 B** |

## Hash ErrRate

``` ini
LCGWithFNV1 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:211.8174ms Count:100000 ErrRate:93.921 ErrTotal:93921 Final ErrRate:100.001
LCGWithFNV1a Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:189.4414ms Count:100000 ErrRate:93.929 ErrTotal:93929 Final ErrRate:100.001
LCGModifiedFNV1 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:57.8939ms Count:100000 ErrRate:93.944 ErrTotal:93944 Final ErrRate:100.001
RNGWithFNV1 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:249.5932ms Count:100000 ErrRate:0.163 ErrTotal:163 Final ErrRate:0.955
RNGWithFNV1a Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:287.2725ms Count:100000 ErrRate:0.180 ErrTotal:180 Final ErrRate:0.966
RNGModifiedFNV1 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:195.7422ms Count:100000 ErrRate:0.150 ErrTotal:150 Final ErrRate:0.957
CRC32 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:777.4468ms Count:100000 ErrRate:0.177 ErrTotal:177 Final ErrRate:1.018
CRC32u Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:892.5557ms Count:100000 ErrRate:0.163 ErrTotal:163 Final ErrRate:0.988
Adler32 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:412.0247ms Count:100000 ErrRate:10.168 ErrTotal:10168 Final ErrRate:23.128
Murmur2 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:391.489ms Count:100000 ErrRate:0.148 ErrTotal:148 Final ErrRate:1.032
Murmur3 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:328.3029ms Count:100000 ErrRate:0.180 ErrTotal:180 Final ErrRate:0.977
Murmur3KirschMitzenmacher Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:87.3783ms Count:100000 ErrRate:0.152 ErrTotal:152 Final ErrRate:1.064
SHA1 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:334.3553ms Count:100000 ErrRate:0.187 ErrTotal:187 Final ErrRate:0.954
SHA256 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:203.8082ms Count:100000 ErrRate:0.187 ErrTotal:187 Final ErrRate:0.980
SHA384 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:291.8822ms Count:100000 ErrRate:0.157 ErrTotal:157 Final ErrRate:1.008
SHA512 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:292.8556ms Count:100000 ErrRate:0.151 ErrTotal:151 Final ErrRate:0.958
XXHash32 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:82.8759ms Count:100000 ErrRate:0.170 ErrTotal:170 Final ErrRate:1.017
XXHash64 Capacity:958506,Hashes:7,ExpectedElements:100000,ErrorRate:0.01
 Speed:42.8872ms Count:100000 ErrRate:0.153 ErrTotal:153 Final ErrRate:1.040
```
