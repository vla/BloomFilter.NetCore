# BloomFilter.NetCore

[![Build Status](https://travis-ci.org/vla/BloomFilter.NetCore.svg?branch=master)](https://travis-ci.org/vla/BloomFilter.NetCore)

Library  Bloom filters in C#


Packages & Status
---

Package  | NuGet         |
-------- | :------------ |
|**BloomFilter.NetCore**|[![NuGet package](https://buildstats.info/nuget/BloomFilter.NetCore)](https://www.nuget.org/packages/BloomFilter.NetCore)
|**BloomFilter.Redis.NetCore**|[![NuGet package](https://buildstats.info/nuget/BloomFilter.Redis.NetCore)](https://www.nuget.org/packages/BloomFilter.Redis.NetCore)

Benchmark
---

``` ini

ExpectedElements 1000000
ErrRate 1%

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.316 (1809/October2018Update/Redstone5)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.101
  [Host] : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT
  Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0
  Core   : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT


```


| Method |  Job | Runtime | DataSize |               Method |       Mean |      Error |     StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------- |----- |-------- |--------- |--------------------- |-----------:|-----------:|-----------:|------------:|------------:|------------:|--------------------:|
|    **Add** |  **Clr** |     **Clr** |       **64** |          **LCGWithFNV1** |   **110.5 ns** |  **0.5679 ns** |  **0.5034 ns** |      **0.0132** |           **-** |           **-** |                **56 B** |
|    Add | Core |    Core |       64 |          LCGWithFNV1 |   110.3 ns |  0.4867 ns |  0.4064 ns |      0.0132 |           - |           - |                56 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |         **LCGWithFNV1a** |   **110.7 ns** |  **0.5415 ns** |  **0.4521 ns** |      **0.0132** |           **-** |           **-** |                **56 B** |
|    Add | Core |    Core |       64 |         LCGWithFNV1a |   110.4 ns |  0.1298 ns |  0.1150 ns |      0.0132 |           - |           - |                56 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |      **LCGModifiedFNV1** |   **112.6 ns** |  **0.0429 ns** |  **0.0380 ns** |      **0.0132** |           **-** |           **-** |                **56 B** |
|    Add | Core |    Core |       64 |      LCGModifiedFNV1 |   112.5 ns |  0.0597 ns |  0.0529 ns |      0.0132 |           - |           - |                56 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |          **RNGWithFNV1** |   **656.5 ns** |  **2.9339 ns** |  **2.7444 ns** |      **0.0801** |           **-** |           **-** |               **336 B** |
|    Add | Core |    Core |       64 |          RNGWithFNV1 |   494.1 ns |  1.7143 ns |  1.6036 ns |      0.0801 |           - |           - |               336 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |         **RNGWithFNV1a** |   **647.2 ns** |  **1.5238 ns** |  **1.4253 ns** |      **0.0801** |           **-** |           **-** |               **336 B** |
|    Add | Core |    Core |       64 |         RNGWithFNV1a |   495.1 ns |  0.5436 ns |  0.4819 ns |      0.0801 |           - |           - |               336 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |      **RNGModifiedFNV1** |   **652.1 ns** |  **7.1274 ns** |  **6.3182 ns** |      **0.0801** |           **-** |           **-** |               **336 B** |
|    Add | Core |    Core |       64 |      RNGModifiedFNV1 |   497.7 ns |  1.6453 ns |  1.5390 ns |      0.0801 |           - |           - |               336 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |                **CRC32** | **1,340.2 ns** |  **0.5047 ns** |  **0.4474 ns** |      **0.0420** |           **-** |           **-** |               **184 B** |
|    Add | Core |    Core |       64 |                CRC32 | 1,331.4 ns |  1.5058 ns |  1.4086 ns |      0.0420 |           - |           - |               184 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |               **CRC32u** | **1,639.8 ns** |  **0.2933 ns** |  **0.2290 ns** |      **0.0420** |           **-** |           **-** |               **184 B** |
|    Add | Core |    Core |       64 |               CRC32u | 1,633.9 ns |  0.5485 ns |  0.4283 ns |      0.0420 |           - |           - |               184 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |              **Adler32** |   **527.2 ns** |  **0.4204 ns** |  **0.3933 ns** |      **0.0429** |           **-** |           **-** |               **184 B** |
|    Add | Core |    Core |       64 |              Adler32 |   548.3 ns |  0.5545 ns |  0.4630 ns |      0.0429 |           - |           - |               184 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |              **Murmur2** |   **293.8 ns** |  **0.0719 ns** |  **0.0673 ns** |      **0.0129** |           **-** |           **-** |                **56 B** |
|    Add | Core |    Core |       64 |              Murmur2 |   285.4 ns |  0.2463 ns |  0.2304 ns |      0.0129 |           - |           - |                56 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |              **Murmur3** |   **568.8 ns** |  **0.3037 ns** |  **0.2536 ns** |      **0.0124** |           **-** |           **-** |                **56 B** |
|    Add | Core |    Core |       64 |              Murmur3 |   329.7 ns |  0.3867 ns |  0.3617 ns |      0.0129 |           - |           - |                56 B |
|    **Add** |  **Clr** |     **Clr** |       **64** | **Murmu(...)acher [25]** |   **214.0 ns** |  **0.1077 ns** |  **0.1007 ns** |      **0.0131** |           **-** |           **-** |                **56 B** |
|    Add | Core |    Core |       64 | Murmu(...)acher [25] |   154.4 ns |  0.1124 ns |  0.1051 ns |      0.0131 |           - |           - |                56 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |                 **SHA1** | **5,110.7 ns** | **17.4964 ns** | **16.3662 ns** |      **0.2289** |           **-** |           **-** |               **968 B** |
|    Add | Core |    Core |       64 |                 SHA1 | 3,355.8 ns | 16.0957 ns | 15.0559 ns |      0.1907 |           - |           - |               808 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |               **SHA256** | **9,560.0 ns** | **54.3103 ns** | **50.8019 ns** |      **0.4425** |           **-** |           **-** |              **1912 B** |
|    Add | Core |    Core |       64 |               SHA256 | 1,818.3 ns |  6.0166 ns |  4.6974 ns |      0.1106 |           - |           - |               472 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |               **SHA384** | **7,981.7 ns** | **26.4112 ns** | **24.7051 ns** |      **0.3204** |           **-** |           **-** |              **1392 B** |
|    Add | Core |    Core |       64 |               SHA384 | 3,388.9 ns | 11.6346 ns |  9.7154 ns |      0.1869 |           - |           - |               792 B |
|    **Add** |  **Clr** |     **Clr** |       **64** |               **SHA512** | **7,707.8 ns** | **33.2082 ns** | **31.0630 ns** |      **0.3357** |           **-** |           **-** |              **1456 B** |
|    Add | Core |    Core |       64 |               SHA512 | 1,737.9 ns | 40.5610 ns | 39.8363 ns |      0.1431 |           - |           - |               600 B |
