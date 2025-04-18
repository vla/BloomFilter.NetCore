# BloomFilter.NetCore

The Bloom filter is a probabilistic set data structure which is very small. This is achieved by allowing false positives with some probability p. It has an add and contains operation which both are very fast (time complexity O(1)).

There are 2 types of Bloom filters in the Bloom filter library:

* Regular Bloom filter, a regular in-memory Bloom filter (FilterMemory)
* Redis Bloom Filter, a Redis-backed Bloom filter which can be concurrently used by different applications (FilterRedis | FilterFreeRedis | FilterCSRedis)

## Packages & Status

Package  | NuGet         |
-------- | :------------ |
|**BloomFilter.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.NetCore)
|**BloomFilter.Redis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.Redis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.Redis.NetCore)
|**BloomFilter.CSRedis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.CSRedis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.CSRedis.NetCore)
|**BloomFilter.FreeRedis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.FreeRedis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.FreeRedis.NetCore)
|**BloomFilter.EasyCaching.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.EasyCaching.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.EasyCaching.NetCore)

## Features

* Configuration of all parameters: Bit-Array size m, number of hash functions k
* Automatic configuration given the tolerable false positive rate p and expected elements n
* Choice among different hash functions: the better (i.e. uniformly distributed) the hash function, the more accurate the Bloom filter but the better the hash function usually the slower it is -> choose from about 10-20 optimized hash functions, e.g. CRC, MD5, SHA, Murmur, LCGs, xxHash etc. or use a custom one
* Generation of the Bloom filter is always fast
* Implementation of [rejection sampling](http://en.wikipedia.org/wiki/Rejection_sampling) and chaining of hash values taking into account the [avalanche effect](http://en.wikipedia.org/wiki/Avalanche_effect) (higher hash quality)
* Concurrency: the Bloom filter can be accessed by many clients simultaneously without multi-user anomalies and performance degradation

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

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1778/22H2/2022Update/SunValley2)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.304
  [Host]     : .NET 7.0.7 (7.0.723.27404), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.7 (7.0.723.27404), X64 RyuJIT AVX2
```

| Method | DataSize |           Method |            Mean |        Error |       StdDev |          Median |             Min |             Max | Rank |   Gen0 | Allocated |
|------- |--------- |----------------- |----------------:|-------------:|-------------:|----------------:|----------------:|----------------:|-----:|-------:|----------:|
|    **Add** |       **64** |      **LCGWithFNV1** |        **91.27 ns** |     **1.792 ns** |     **2.134 ns** |        **90.38 ns** |        **88.40 ns** |        **96.90 ns** |   **11** | **0.0048** |      **80 B** |
|    **Add** |       **64** |     **LCGWithFNV1a** |        **90.88 ns** |     **0.638 ns** |     **0.498 ns** |        **90.91 ns** |        **89.75 ns** |        **91.60 ns** |   **11** | **0.0048** |      **80 B** |
|    **Add** |       **64** |  **LCGModifiedFNV1** |        **91.80 ns** |     **1.009 ns** |     **0.895 ns** |        **91.50 ns** |        **90.96 ns** |        **93.72 ns** |   **11** | **0.0048** |      **80 B** |
|    **Add** |       **64** |      **RNGWithFNV1** |       **445.32 ns** |     **8.463 ns** |     **9.747 ns** |       **443.96 ns** |       **433.96 ns** |       **471.98 ns** |   **18** | **0.0229** |     **384 B** |
|    **Add** |       **64** |     **RNGWithFNV1a** |       **494.80 ns** |     **7.796 ns** |     **6.911 ns** |       **494.26 ns** |       **487.26 ns** |       **508.36 ns** |   **21** | **0.0229** |     **384 B** |
|    **Add** |       **64** |  **RNGModifiedFNV1** |       **461.74 ns** |     **4.737 ns** |     **4.199 ns** |       **461.52 ns** |       **455.88 ns** |       **471.08 ns** |   **19** | **0.0229** |     **384 B** |
|    **Add** |       **64** |            **CRC32** |       **145.63 ns** |     **1.528 ns** |     **1.429 ns** |       **145.35 ns** |       **143.68 ns** |       **148.68 ns** |   **13** | **0.0196** |     **328 B** |
|    **Add** |       **64** |            **CRC64** |        **38.83 ns** |     **0.399 ns** |     **0.333 ns** |        **38.93 ns** |        **38.32 ns** |        **39.38 ns** |    **3** | **0.0048** |      **80 B** |
|    **Add** |       **64** |          **Adler32** |       **150.07 ns** |     **0.664 ns** |     **0.589 ns** |       **150.24 ns** |       **148.82 ns** |       **150.83 ns** |   **14** | **0.0200** |     **336 B** |
|    **Add** |       **64** |          **Murmur3** |        **70.98 ns** |     **1.108 ns** |     **1.036 ns** |        **70.43 ns** |        **69.88 ns** |        **73.06 ns** |    **6** | **0.0048** |      **80 B** |
|    **Add** |       **64** |  **Murmur32BitsX86** |        **71.19 ns** |     **0.727 ns** |     **0.607 ns** |        **71.02 ns** |        **70.33 ns** |        **72.40 ns** |    **6** | **0.0048** |      **80 B** |
|    **Add** |       **64** | **Murmur128BitsX64** |        **80.15 ns** |     **0.783 ns** |     **0.653 ns** |        **80.26 ns** |        **78.99 ns** |        **81.30 ns** |    **8** | **0.0072** |     **120 B** |
|    **Add** |       **64** | **Murmur128BitsX86** |        **82.73 ns** |     **1.211 ns** |     **1.011 ns** |        **82.80 ns** |        **81.47 ns** |        **84.73 ns** |    **9** | **0.0072** |     **120 B** |
|    **Add** |       **64** |             **SHA1** |     **1,045.67 ns** |     **6.411 ns** |     **5.997 ns** |     **1,044.74 ns** |     **1,037.64 ns** |     **1,057.40 ns** |   **25** | **0.0267** |     **464 B** |
|    **Add** |       **64** |           **SHA256** |       **922.30 ns** |     **4.478 ns** |     **3.739 ns** |       **920.17 ns** |       **917.46 ns** |       **929.13 ns** |   **24** | **0.0296** |     **496 B** |
|    **Add** |       **64** |           **SHA384** |     **1,173.67 ns** |     **5.050 ns** |     **3.942 ns** |     **1,172.66 ns** |     **1,168.47 ns** |     **1,181.55 ns** |   **26** | **0.0267** |     **456 B** |
|    **Add** |       **64** |           **SHA512** |     **1,368.20 ns** |    **10.967 ns** |     **9.722 ns** |     **1,366.36 ns** |     **1,354.64 ns** |     **1,388.86 ns** |   **28** | **0.0286** |     **504 B** |
|    **Add** |       **64** |         **XXHash32** |        **73.15 ns** |     **0.526 ns** |     **0.466 ns** |        **73.22 ns** |        **72.43 ns** |        **74.02 ns** |    **7** | **0.0048** |      **80 B** |
|    **Add** |       **64** |         **XXHash64** |        **50.62 ns** |     **0.756 ns** |     **0.670 ns** |        **50.36 ns** |        **49.88 ns** |        **51.99 ns** |    **4** | **0.0048** |      **80 B** |
|    **Add** |       **64** |          **XXHash3** |        **33.14 ns** |     **0.295 ns** |     **0.276 ns** |        **33.09 ns** |        **32.75 ns** |        **33.78 ns** |    **1** | **0.0048** |      **80 B** |
|    **Add** |       **64** |        **XXHash128** |        **36.01 ns** |     **0.673 ns** |     **0.749 ns** |        **36.00 ns** |        **34.62 ns** |        **37.40 ns** |    **2** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |      **LCGWithFNV1** |       **899.25 ns** |     **1.286 ns** |     **1.140 ns** |       **899.00 ns** |       **897.38 ns** |       **901.74 ns** |   **23** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |     **LCGWithFNV1a** |       **900.59 ns** |     **3.196 ns** |     **2.990 ns** |       **899.46 ns** |       **897.27 ns** |       **906.06 ns** |   **23** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |  **LCGModifiedFNV1** |       **898.96 ns** |     **1.230 ns** |     **1.090 ns** |       **898.95 ns** |       **896.35 ns** |       **900.95 ns** |   **23** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |      **RNGWithFNV1** |     **1,258.57 ns** |     **2.607 ns** |     **2.311 ns** |     **1,257.83 ns** |     **1,255.55 ns** |     **1,263.52 ns** |   **27** | **0.0229** |     **384 B** |
|    **Add** |     **1024** |     **RNGWithFNV1a** |     **1,262.25 ns** |     **9.823 ns** |     **8.203 ns** |     **1,259.77 ns** |     **1,255.89 ns** |     **1,279.38 ns** |   **27** | **0.0229** |     **384 B** |
|    **Add** |     **1024** |  **RNGModifiedFNV1** |     **1,273.95 ns** |     **7.871 ns** |     **6.572 ns** |     **1,273.07 ns** |     **1,263.13 ns** |     **1,286.95 ns** |   **27** | **0.0229** |     **384 B** |
|    **Add** |     **1024** |            **CRC32** |       **549.69 ns** |     **5.392 ns** |     **4.780 ns** |       **547.87 ns** |       **545.01 ns** |       **560.14 ns** |   **22** | **0.0191** |     **328 B** |
|    **Add** |     **1024** |            **CRC64** |        **88.11 ns** |     **0.844 ns** |     **0.790 ns** |        **87.76 ns** |        **87.25 ns** |        **89.80 ns** |   **10** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |          **Adler32** |       **336.74 ns** |     **6.487 ns** |     **5.417 ns** |       **339.17 ns** |       **327.21 ns** |       **345.29 ns** |   **16** | **0.0200** |     **336 B** |
|    **Add** |     **1024** |          **Murmur3** |       **478.67 ns** |     **2.061 ns** |     **1.928 ns** |       **478.31 ns** |       **476.10 ns** |       **482.93 ns** |   **20** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |  **Murmur32BitsX86** |       **489.07 ns** |     **9.783 ns** |    **20.203 ns** |       **478.96 ns** |       **476.13 ns** |       **558.53 ns** |   **20** | **0.0048** |      **80 B** |
|    **Add** |     **1024** | **Murmur128BitsX64** |       **235.87 ns** |     **1.664 ns** |     **1.557 ns** |       **236.20 ns** |       **233.30 ns** |       **238.38 ns** |   **15** | **0.0072** |     **120 B** |
|    **Add** |     **1024** | **Murmur128BitsX86** |       **334.17 ns** |     **0.569 ns** |     **0.475 ns** |       **334.24 ns** |       **333.45 ns** |       **335.09 ns** |   **16** | **0.0072** |     **120 B** |
|    **Add** |     **1024** |             **SHA1** |     **4,539.64 ns** |     **2.843 ns** |     **2.374 ns** |     **4,539.24 ns** |     **4,535.17 ns** |     **4,543.87 ns** |   **32** | **0.0305** |     **544 B** |
|    **Add** |     **1024** |           **SHA256** |     **1,747.90 ns** |     **4.390 ns** |     **4.106 ns** |     **1,748.48 ns** |     **1,739.81 ns** |     **1,754.14 ns** |   **29** | **0.0286** |     **496 B** |
|    **Add** |     **1024** |           **SHA384** |     **2,208.86 ns** |     **3.754 ns** |     **3.328 ns** |     **2,208.17 ns** |     **2,204.62 ns** |     **2,215.67 ns** |   **30** | **0.0267** |     **456 B** |
|    **Add** |     **1024** |           **SHA512** |     **2,390.80 ns** |     **4.451 ns** |     **4.164 ns** |     **2,390.12 ns** |     **2,385.87 ns** |     **2,400.78 ns** |   **31** | **0.0267** |     **504 B** |
|    **Add** |     **1024** |         **XXHash32** |       **421.10 ns** |     **0.343 ns** |     **0.304 ns** |       **421.19 ns** |       **420.56 ns** |       **421.45 ns** |   **17** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |         **XXHash64** |       **140.22 ns** |     **0.233 ns** |     **0.218 ns** |       **140.26 ns** |       **139.91 ns** |       **140.63 ns** |   **12** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |          **XXHash3** |        **67.50 ns** |     **0.134 ns** |     **0.112 ns** |        **67.48 ns** |        **67.32 ns** |        **67.75 ns** |    **5** | **0.0048** |      **80 B** |
|    **Add** |     **1024** |        **XXHash128** |        **70.36 ns** |     **0.220 ns** |     **0.206 ns** |        **70.38 ns** |        **70.03 ns** |        **70.68 ns** |    **6** | **0.0048** |      **80 B** |
|    **Add** |  **1048576** |      **LCGWithFNV1** |   **865,507.14 ns** |    **33.403 ns** |    **27.893 ns** |   **865,515.92 ns** |   **865,431.84 ns** |   **865,536.13 ns** |   **43** |      **-** |      **80 B** |
|    **Add** |  **1048576** |     **LCGWithFNV1a** |   **865,569.19 ns** |    **37.885 ns** |    **33.584 ns** |   **865,566.46 ns** |   **865,513.28 ns** |   **865,628.71 ns** |   **43** |      **-** |      **80 B** |
|    **Add** |  **1048576** |  **LCGModifiedFNV1** |   **865,544.18 ns** |    **49.942 ns** |    **44.272 ns** |   **865,526.27 ns** |   **865,493.55 ns** |   **865,652.54 ns** |   **43** |      **-** |      **80 B** |
|    **Add** |  **1048576** |      **RNGWithFNV1** |   **866,009.20 ns** |    **58.792 ns** |    **52.118 ns** |   **865,989.70 ns** |   **865,952.73 ns** |   **866,125.00 ns** |   **43** |      **-** |     **384 B** |
|    **Add** |  **1048576** |     **RNGWithFNV1a** |   **866,010.55 ns** |    **65.896 ns** |    **61.639 ns** |   **866,001.37 ns** |   **865,898.73 ns** |   **866,147.17 ns** |   **43** |      **-** |     **384 B** |
|    **Add** |  **1048576** |  **RNGModifiedFNV1** |   **866,024.55 ns** |    **52.725 ns** |    **46.739 ns** |   **866,033.84 ns** |   **865,936.52 ns** |   **866,088.77 ns** |   **43** |      **-** |     **384 B** |
|    **Add** |  **1048576** |            **CRC32** |   **405,358.53 ns** |   **190.366 ns** |   **168.755 ns** |   **405,333.28 ns** |   **405,092.04 ns** |   **405,753.81 ns** |   **41** |      **-** |     **328 B** |
|    **Add** |  **1048576** |            **CRC64** |    **56,321.74 ns** |    **23.680 ns** |    **19.774 ns** |    **56,315.30 ns** |    **56,303.48 ns** |    **56,370.02 ns** |   **35** |      **-** |      **80 B** |
|    **Add** |  **1048576** |          **Adler32** |   **183,236.21 ns** |   **307.783 ns** |   **287.900 ns** |   **183,033.96 ns** |   **182,984.23 ns** |   **183,816.09 ns** |   **38** |      **-** |     **336 B** |
|    **Add** |  **1048576** |          **Murmur3** |   **441,874.65 ns** |   **120.222 ns** |   **112.456 ns** |   **441,926.81 ns** |   **441,729.10 ns** |   **442,070.12 ns** |   **42** |      **-** |      **80 B** |
|    **Add** |  **1048576** |  **Murmur32BitsX86** |   **441,850.23 ns** |   **132.677 ns** |   **117.615 ns** |   **441,812.06 ns** |   **441,735.35 ns** |   **442,130.66 ns** |   **42** |      **-** |      **80 B** |
|    **Add** |  **1048576** | **Murmur128BitsX64** |   **163,915.44 ns** |    **77.542 ns** |    **72.533 ns** |   **163,879.64 ns** |   **163,843.26 ns** |   **164,037.11 ns** |   **37** |      **-** |     **120 B** |
|    **Add** |  **1048576** | **Murmur128BitsX86** |   **274,620.93 ns** |   **185.247 ns** |   **173.280 ns** |   **274,522.12 ns** |   **274,446.83 ns** |   **274,957.18 ns** |   **39** |      **-** |     **120 B** |
|    **Add** |  **1048576** |             **SHA1** | **3,381,425.73 ns** |   **706.447 ns** |   **626.247 ns** | **3,381,191.80 ns** | **3,380,729.69 ns** | **3,382,828.91 ns** |   **47** |      **-** |     **546 B** |
|    **Add** |  **1048576** |           **SHA256** |   **874,830.84 ns** |    **33.305 ns** |    **31.153 ns** |   **874,826.37 ns** |   **874,775.29 ns** |   **874,903.61 ns** |   **44** |      **-** |     **496 B** |
|    **Add** |  **1048576** |           **SHA384** | **1,306,967.72 ns** |   **831.669 ns** |   **777.943 ns** | **1,307,006.64 ns** | **1,305,832.23 ns** | **1,308,288.67 ns** |   **45** |      **-** |     **457 B** |
|    **Add** |  **1048576** |           **SHA512** | **2,610,481.14 ns** | **1,360.749 ns** | **1,206.269 ns** | **2,610,458.01 ns** | **2,608,700.00 ns** | **2,612,751.56 ns** |   **46** |      **-** |     **626 B** |
|    **Add** |  **1048576** |         **XXHash32** |   **382,378.24 ns** |    **96.264 ns** |    **85.336 ns** |   **382,386.87 ns** |   **382,255.91 ns** |   **382,521.44 ns** |   **40** |      **-** |      **80 B** |
|    **Add** |  **1048576** |         **XXHash64** |   **100,570.79 ns** |    **15.037 ns** |    **12.557 ns** |   **100,570.25 ns** |   **100,551.87 ns** |   **100,591.17 ns** |   **36** |      **-** |      **80 B** |
|    **Add** |  **1048576** |          **XXHash3** |    **30,258.92 ns** |    **66.019 ns** |    **61.754 ns** |    **30,218.56 ns** |    **30,196.84 ns** |    **30,357.71 ns** |   **33** |      **-** |      **80 B** |
|    **Add** |  **1048576** |        **XXHash128** |    **33,778.68 ns** |    **22.719 ns** |    **20.139 ns** |    **33,779.05 ns** |    **33,745.28 ns** |    **33,818.32 ns** |   **34** |      **-** |      **80 B** |


## Hash ErrRate

Count:100000 Capacity:958506 Hashes:7 ExpectedElements:100000 ErrorRate:0.01

``` ini
LCGWithFNV1
 Speed:69.1082ms ErrRate:93.942% ErrTotal:93942 Final ErrRate:100.000%
LCGWithFNV1a
 Speed:82.353ms ErrRate:93.928% ErrTotal:93928 Final ErrRate:100.000%
LCGModifiedFNV1
 Speed:58.9601ms ErrRate:93.926% ErrTotal:93926 Final ErrRate:100.000%
RNGWithFNV1
 Speed:218.8725ms ErrRate:0.156% ErrTotal:156 Final ErrRate:1.034%
RNGWithFNV1a
 Speed:219.4473ms ErrRate:0.153% ErrTotal:153 Final ErrRate:1.071%
RNGModifiedFNV1
 Speed:221.0097ms ErrRate:0.167% ErrTotal:167 Final ErrRate:1.063%
CRC32
 Speed:74.8524ms ErrRate:0.183% ErrTotal:183 Final ErrRate:1.018%
CRC64
 Speed:22.3531ms ErrRate:0.173% ErrTotal:173 Final ErrRate:0.925%
Adler32
 Speed:86.8095ms ErrRate:10.085% ErrTotal:10085 Final ErrRate:23.404%
Murmur3
 Speed:42.2731ms ErrRate:0.156% ErrTotal:156 Final ErrRate:0.973%
Murmur32BitsX86
 Speed:42.6358ms ErrRate:0.156% ErrTotal:156 Final ErrRate:0.973%
Murmur128BitsX64
 Speed:32.2329ms ErrRate:0.160% ErrTotal:160 Final ErrRate:1.021%
Murmur128BitsX86
 Speed:36.654ms ErrRate:0.169% ErrTotal:169 Final ErrRate:1.038%
SHA1
 Speed:274.6806ms ErrRate:0.175% ErrTotal:175 Final ErrRate:0.952%
SHA256
 Speed:209.7873ms ErrRate:0.170% ErrTotal:170 Final ErrRate:0.984%
SHA384
 Speed:315.9428ms ErrRate:0.165% ErrTotal:165 Final ErrRate:0.988%
SHA512
 Speed:322.2861ms ErrRate:0.171% ErrTotal:171 Final ErrRate:0.998%
XXHash32
 Speed:41.2861ms ErrRate:0.163% ErrTotal:163 Final ErrRate:1.035%
XXHash64
 Speed:23.5383ms ErrRate:0.149% ErrTotal:149 Final ErrRate:1.054%
XXHash3
 Speed:19.8399ms ErrRate:0.157% ErrTotal:157 Final ErrRate:0.966%
XXHash128
 Speed:19.7745ms ErrRate:0.155% ErrTotal:155 Final ErrRate:1.027%
```
