# BloomFilter.NetCore

[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6.0%20|%207.0%20|%208.0%20|%209.0%20|%2010.0-blue.svg)](https://dotnet.microsoft.com/)

A high-performance, feature-complete Bloom filter library for .NET, supporting both in-memory and distributed Redis backends.

[中文文档](README.zh-CN.md)

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Packages & Status](#packages--status)
- [Architecture](#architecture)
- [Core Functionality](#core-functionality)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Usage Examples](#usage-examples)
  - [In-Memory Mode](#in-memory-mode)
  - [Dependency Injection](#dependency-injection)
  - [Redis Distributed Mode](#redis-distributed-mode)
- [Hash Algorithms](#hash-algorithms)
- [Performance Benchmarks](#performance-benchmarks)
- [Advanced Usage](#advanced-usage)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)

## Overview

BloomFilter.NetCore is an enterprise-grade Bloom filter library designed for the .NET ecosystem. A Bloom filter is a space-efficient probabilistic data structure used to test whether an element is a member of a set. Its core characteristics are:

- **Space Efficient**: Extremely small memory footprint compared to traditional HashSets
- **O(1) Time Complexity**: Both add and query operations execute in constant time
- **Probabilistic**: May return false positives but never false negatives

This project provides two major implementation types:

1. **In-Memory Bloom Filter (FilterMemory)**: BitArray-based in-memory implementation, suitable for single-process scenarios
2. **Distributed Bloom Filter (FilterRedis series)**: Redis-backed distributed implementation, supports concurrent access from multiple applications

### Primary Use Cases

- **Cache Penetration Protection**: Prevent malicious queries for non-existent data from bypassing cache
- **Deduplication**: URL deduplication, email deduplication, user ID deduplication, etc.
- **Recommendation Systems**: Check if a user has seen specific content
- **Web Crawlers**: Check if URLs have been crawled
- **Distributed Systems**: Share state checks across multiple service instances
- **Big Data**: Existence checks for massive datasets

## Key Features

### 🎯 Flexible Configuration

- **Fully Configurable Parameters**: Bit array size (m), number of hash functions (k)
- **Automatic Parameter Calculation**: Automatically calculate optimal parameters based on tolerable false positive rate (p) and expected element count (n)
- **20+ Hash Algorithms**: Support for CRC, MD5, SHA, Murmur, LCGs, xxHash, or custom algorithms

### ⚡ High Performance

- **Fast Generation**: Bloom filter generation and operations are extremely fast
- **Optimized Implementation**: Uses Span<T>, ReadOnlyMemory<T> for zero-copy operations
- **Unsafe Code Optimization**: Uses unsafe code blocks in performance-critical paths
- **Rejection Sampling**: Implements rejection sampling and hash chaining, considering avalanche effect for improved hash quality

### 🔒 Concurrency Safe

- **Thread-Safe**: Uses AsyncLock mechanism for safe multi-threaded concurrent access
- **Async Support**: Comprehensive async/await support with async versions of all operations
- **Distributed Locking**: Redis implementations support concurrent access across applications

### 🌐 Multiple Backend Support

- **StackExchange.Redis**: Officially recommended Redis client
- **CSRedisCore**: High-performance Redis client
- **FreeRedis**: Lightweight Redis client
- **EasyCaching**: Supports EasyCaching abstraction layer, switchable cache providers

### 📦 Modern .NET Support

- **Multi-Framework Support**: net462, netstandard2.0, net6.0, net7.0, net8.0, net9.0, net10.0
- **Dependency Injection**: Native support for Microsoft.Extensions.DependencyInjection
- **Nullable Reference Types**: Enabled for improved code safety

## Packages & Status

| Package | NuGet | Description |
|---------|-------|-------------|
|**BloomFilter.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.NetCore)| Core package with in-memory Bloom filter |
|**BloomFilter.Redis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.Redis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.Redis.NetCore)| StackExchange.Redis implementation |
|**BloomFilter.CSRedis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.CSRedis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.CSRedis.NetCore)| CSRedisCore implementation |
|**BloomFilter.FreeRedis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.FreeRedis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.FreeRedis.NetCore)| FreeRedis implementation |
|**BloomFilter.EasyCaching.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.EasyCaching.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.EasyCaching.NetCore)| EasyCaching integration |

## Architecture

### Core Interface Layer

```
IBloomFilter (Interface)
    ├── Add / AddAsync           - Add elements
    ├── Contains / ContainsAsync - Check elements
    ├── All / AllAsync           - Batch check
    ├── Clear / ClearAsync       - Clear filter
    └── ComputeHash              - Compute hash values
```

### Implementation Hierarchy

```
Filter (Abstract Base Class)
    ├── FilterMemory (In-Memory)
    │   └── Uses BitArray storage
    │
    └── Redis Series (Distributed)
        ├── FilterRedis (StackExchange.Redis)
        ├── FilterCSRedis (CSRedisCore)
        ├── FilterFreeRedis (FreeRedis)
        └── FilterEasyCachingRedis (EasyCaching)
```

### Configuration System

```
BloomFilterOptions
    ├── FilterMemoryOptions      - In-memory mode configuration
    ├── FilterRedisOptions       - StackExchange.Redis configuration
    ├── FilterCSRedisOptions     - CSRedisCore configuration
    ├── FilterFreeRedisOptions   - FreeRedis configuration
    └── FilterEasyCachingOptions - EasyCaching configuration
```

## Core Functionality

### Mathematical Model

BloomFilter.NetCore implements the complete Bloom filter mathematical model:

#### 1. Optimal Bit Array Size (m)

Given expected element count `n` and false positive rate `p`, calculate optimal bit array size:

```
m = -(n * ln(p)) / (ln(2)^2)
```

#### 2. Optimal Number of Hash Functions (k)

Given element count `n` and bit array size `m`, calculate optimal number of hash functions:

```
k = (m / n) * ln(2)
```

#### 3. Actual False Positive Rate (p)

Given inserted element count, number of hash functions, and bit array size, calculate actual false positive rate:

```
p = (1 - e^(-k*n/m))^k
```

These calculations are provided by static methods in the `Filter` base class:

```csharp
// Calculate optimal bit array size
long m = Filter.BestM(expectedElements, errorRate);

// Calculate optimal number of hash functions
int k = Filter.BestK(expectedElements, capacity);

// Calculate optimal element count
long n = Filter.BestN(hashes, capacity);

// Calculate actual false positive rate
double p = Filter.BestP(hashes, capacity, insertedElements);
```

### Storage Mechanisms

#### In-Memory Storage

- **BitArray**: Uses .NET's BitArray as underlying storage
- **Bucketing Strategy**: Automatically splits into multiple BitArrays when capacity exceeds 2GB (MaxInt = 2,147,483,640)
- **Serialization Support**: Supports serialization/deserialization for persistence or transfer

#### Redis Storage

- **SETBIT/GETBIT**: Uses Redis bit operation commands
- **Distributed Access**: Multiple application instances can concurrently access the same filter
- **Persistence**: Leverages Redis persistence mechanisms for data safety

### Concurrency Control

```csharp
// AsyncLock ensures thread safety
public class AsyncLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async ValueTask<IDisposable> LockAsync()
    {
        await _semaphore.WaitAsync();
        return new Release(_semaphore);
    }
}
```

## Installation

### Install via NuGet

**In-Memory Mode (Core Package):**

```bash
dotnet add package BloomFilter.NetCore
```

**Redis Distributed Mode (Choose One):**

```bash
# StackExchange.Redis
dotnet add package BloomFilter.Redis.NetCore

# CSRedisCore
dotnet add package BloomFilter.CSRedis.NetCore

# FreeRedis
dotnet add package BloomFilter.FreeRedis.NetCore

# EasyCaching
dotnet add package BloomFilter.EasyCaching.NetCore
```

## Quick Start

### Simplest Example

```csharp
using BloomFilter;

// Create a Bloom filter: expect 10 million elements, 1% false positive rate
var bf = FilterBuilder.Build(10_000_000, 0.01);

// Add elements
bf.Add("user:123");
bf.Add("user:456");

// Check element existence
Console.WriteLine(bf.Contains("user:123")); // True
Console.WriteLine(bf.Contains("user:789")); // False (very small probability of True)

// Clear filter
bf.Clear();
```

### Async Operations

```csharp
// Async add
await bf.AddAsync(Encoding.UTF8.GetBytes("user:123"));

// Async check
bool exists = await bf.ContainsAsync(Encoding.UTF8.GetBytes("user:123"));

// Batch async operations
var users = new[] {
    Encoding.UTF8.GetBytes("user:1"),
    Encoding.UTF8.GetBytes("user:2"),
    Encoding.UTF8.GetBytes("user:3")
};

await bf.AddAsync(users);
var results = await bf.ContainsAsync(users);
```

### Fluent API (New in v3.0)

v3.0 introduces a modern fluent API for building Bloom filters with improved discoverability and expressiveness:

```csharp
// In-Memory Fluent API
var filter = FilterBuilder.Create()
    .WithName("UserFilter")
    .ExpectingElements(10_000_000)
    .WithErrorRate(0.001)
    .UsingHashMethod(HashMethod.XXHash3)
    .BuildInMemory();

// Redis Fluent API (StackExchange.Redis)
var redisFilter = FilterRedisBuilder.Create()
    .WithRedisConnection("localhost:6379")
    .WithRedisKey("bloom:users")
    .WithName("UserFilter")
    .ExpectingElements(10_000_000)
    .WithErrorRate(0.001)
    .BuildRedis();

// CSRedis Fluent API
var csredisFilter = FilterCSRedisBuilder.Create()
    .WithRedisClient(csredisClient)
    .WithRedisKey("bloom:users")
    .ExpectingElements(10_000_000)
    .BuildCSRedis();

// FreeRedis Fluent API
var freeRedisFilter = FilterFreeRedisBuilder.Create()
    .WithRedisClient(redisClient)
    .WithRedisKey("bloom:users")
    .ExpectingElements(10_000_000)
    .BuildFreeRedis();

// EasyCaching Fluent API
var easyCachingFilter = FilterEasyCachingBuilder.Create()
    .WithRedisCachingProvider(provider)
    .WithRedisKey("bloom:users")
    .ExpectingElements(10_000_000)
    .BuildEasyCaching();

// All common configuration methods:
// - WithName(string) - Set filter name
// - ExpectingElements(long) - Set expected element count
// - WithErrorRate(double) - Set false positive rate (0-1)
// - UsingHashMethod(HashMethod) - Use predefined hash algorithm
// - UsingCustomHash(HashFunction) - Use custom hash function
// - WithSerializer(IFilterMemorySerializer) - Set custom serializer (memory only)
```

**Why use Fluent API?**
- 🔍 Better discoverability with IntelliSense
- 📖 More readable and self-documenting code
- ⛓️ Chainable method calls
- 🎯 Type-safe configuration
- ✅ Backward compatible - old static methods still work!

## Usage Examples

### In-Memory Mode

#### Basic Usage

```csharp
using BloomFilter;

public class UserService
{
    // Static shared Bloom filter
    private static readonly IBloomFilter _bloomFilter =
        FilterBuilder.Build(10_000_000, 0.01);

    public void AddUser(string userId)
    {
        // Add user ID
        _bloomFilter.Add(userId);
    }

    public bool MayExistUser(string userId)
    {
        // Check if user may exist
        return _bloomFilter.Contains(userId);
    }
}
```

#### Custom Configuration

```csharp
using BloomFilter;

// Method 1: Specify hash algorithm
var bf1 = FilterBuilder.Build(
    expectedElements: 1_000_000,
    errorRate: 0.001,
    hashMethod: HashMethod.Murmur3
);

// Method 2: Use custom hash function
var hashFunction = new Murmur128BitsX64();
var bf2 = FilterBuilder.Build(
    expectedElements: 1_000_000,
    errorRate: 0.001,
    hashFunction: hashFunction
);

// Method 3: Manually specify parameters (advanced usage)
var bf3 = FilterBuilder.Build(
    capacity: 9585059,      // Bit array size
    hashes: 10,             // Number of hash functions
    hashMethod: HashMethod.XXHash3
);

// Method 4: Use configuration object
var options = new FilterMemoryOptions
{
    Name = "MyFilter",
    ExpectedElements = 5_000_000,
    ErrorRate = 0.01,
    Method = HashMethod.Murmur3
};
var bf4 = FilterBuilder.Build(options);
```

### Dependency Injection

#### ASP.NET Core Integration

```csharp
using BloomFilter;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register Bloom filter service
        services.AddBloomFilter(setupAction =>
        {
            setupAction.UseInMemory(options =>
            {
                options.Name = "MainFilter";
                options.ExpectedElements = 10_000_000;
                options.ErrorRate = 0.01;
                options.Method = HashMethod.Murmur3;
            });
        });

        services.AddControllers();
    }
}

// Use in controller or service
public class UserController : ControllerBase
{
    private readonly IBloomFilter _bloomFilter;

    public UserController(IBloomFilter bloomFilter)
    {
        _bloomFilter = bloomFilter;
    }

    [HttpPost("users/{userId}")]
    public IActionResult CheckUser(string userId)
    {
        if (_bloomFilter.Contains(userId))
        {
            // User may exist, continue to query database
            return Ok("User may exist");
        }
        else
        {
            // User definitely doesn't exist, no need to query database
            return NotFound("User doesn't exist");
        }
    }
}
```

#### Multiple Filter Instances

```csharp
services.AddBloomFilter(setupAction =>
{
    // User filter
    setupAction.UseInMemory(options =>
    {
        options.Name = "UserFilter";
        options.ExpectedElements = 10_000_000;
        options.ErrorRate = 0.01;
    });

    // Email filter
    setupAction.UseInMemory(options =>
    {
        options.Name = "EmailFilter";
        options.ExpectedElements = 5_000_000;
        options.ErrorRate = 0.001;
    });
});

// Use factory to get specific filter
public class MyService
{
    private readonly IBloomFilter _userFilter;
    private readonly IBloomFilter _emailFilter;

    public MyService(IBloomFilterFactory factory)
    {
        _userFilter = factory.Get("UserFilter");
        _emailFilter = factory.Get("EmailFilter");
    }
}
```

### Redis Distributed Mode

#### StackExchange.Redis

```csharp
using BloomFilter;

// Method 1: Direct build
var bf = FilterRedisBuilder.Build(
    redisHost: "localhost:6379",
    name: "DistributedFilter",
    expectedElements: 5_000_000,
    errorRate: 0.001
);

bf.Add("item:123");
Console.WriteLine(bf.Contains("item:123")); // True

// Method 2: Dependency injection
services.AddBloomFilter(setupAction =>
{
    setupAction.UseRedis(new FilterRedisOptions
    {
        Name = "UserFilter",
        RedisKey = "BloomFilter:Users",
        Endpoints = new List<string> { "localhost:6379" },
        Database = 0,
        ExpectedElements = 10_000_000,
        ErrorRate = 0.01,
        Method = HashMethod.Murmur3
    });
});

// Method 3: Advanced configuration (master-slave, sentinel, cluster)
services.AddBloomFilter(setupAction =>
{
    setupAction.UseRedis(new FilterRedisOptions
    {
        Name = "ProductFilter",
        RedisKey = "BloomFilter:Products",
        Endpoints = new List<string>
        {
            "redis-master:6379",
            "redis-slave1:6379",
            "redis-slave2:6379"
        },
        Password = "your-redis-password",
        Ssl = true,
        ConnectTimeout = 5000,
        SyncTimeout = 3000,
        ExpectedElements = 20_000_000,
        ErrorRate = 0.001
    });
});
```

#### CSRedisCore

```csharp
services.AddBloomFilter(setupAction =>
{
    setupAction.UseCSRedis(new FilterCSRedisOptions
    {
        Name = "OrderFilter",
        RedisKey = "BloomFilter:Orders",
        ConnectionStrings = new List<string>
        {
            "localhost:6379,password=123456,defaultDatabase=0,poolsize=50,prefix=myapp:"
        },
        ExpectedElements = 5_000_000,
        ErrorRate = 0.01
    });
});
```

#### FreeRedis

```csharp
services.AddBloomFilter(setupAction =>
{
    setupAction.UseFreeRedis(new FilterFreeRedisOptions
    {
        Name = "CartFilter",
        RedisKey = "BloomFilter:Carts",
        ConnectionStrings = new List<string> { "localhost:6379,password=123456" },
        ExpectedElements = 1_000_000,
        ErrorRate = 0.01
    });
});
```

#### EasyCaching Integration

EasyCaching provides a unified caching abstraction layer, allowing you to easily switch underlying cache implementations:

```csharp
using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// 1. Configure EasyCaching
services.AddEasyCaching(options =>
{
    // Configure Redis provider
    options.UseRedis(config =>
    {
        config.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
        config.DBConfig.Database = 0;
    }, "redis-provider-1");

    // Can configure multiple providers
    options.UseRedis(config =>
    {
        config.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
        config.DBConfig.Database = 1;
    }, "redis-provider-2");
});

// 2. Configure BloomFilter
services.AddBloomFilter(setupAction =>
{
    // Use first Redis provider
    setupAction.UseEasyCachingRedis(new FilterEasyCachingRedisOptions
    {
        Name = "BF1",
        RedisKey = "BloomFilter1",
        ProviderName = "redis-provider-1",
        ExpectedElements = 10_000_000,
        ErrorRate = 0.01
    });

    // Use second Redis provider
    setupAction.UseEasyCachingRedis(new FilterEasyCachingRedisOptions
    {
        Name = "BF2",
        RedisKey = "BloomFilter2",
        ProviderName = "redis-provider-2",
        ExpectedElements = 5_000_000,
        ErrorRate = 0.001
    });
});

var provider = services.BuildServiceProvider();

// Use default filter
var bf = provider.GetService<IBloomFilter>();
bf.Add("value1");

// Use named filter
var factory = provider.GetService<IBloomFilterFactory>();
var bf1 = factory.Get("BF1");
var bf2 = factory.Get("BF2");

bf1.Add("item1");
bf2.Add("item2");
```

### Real-World Application Scenarios

#### 1. Cache Penetration Protection

```csharp
public class ProductService
{
    private readonly IBloomFilter _bloomFilter;
    private readonly ICache _cache;
    private readonly IProductRepository _repository;

    public ProductService(
        IBloomFilter bloomFilter,
        ICache cache,
        IProductRepository repository)
    {
        _bloomFilter = bloomFilter;
        _cache = cache;
        _repository = repository;
    }

    public async Task<Product> GetProductAsync(string productId)
    {
        // First layer: Bloom filter
        if (!_bloomFilter.Contains(productId))
        {
            // Product definitely doesn't exist, return null directly
            return null;
        }

        // Second layer: Cache
        var cached = await _cache.GetAsync<Product>(productId);
        if (cached != null)
        {
            return cached;
        }

        // Third layer: Database
        var product = await _repository.GetByIdAsync(productId);
        if (product != null)
        {
            await _cache.SetAsync(productId, product);
        }

        return product;
    }

    public async Task CreateProductAsync(Product product)
    {
        // Save to database
        await _repository.SaveAsync(product);

        // Add to Bloom filter
        _bloomFilter.Add(product.Id);

        // Update cache
        await _cache.SetAsync(product.Id, product);
    }
}
```

#### 2. URL Deduplication (Web Crawler)

```csharp
public class WebCrawler
{
    private readonly IBloomFilter _visitedUrls;
    private readonly Queue<string> _urlQueue;

    public WebCrawler(IBloomFilter bloomFilter)
    {
        _visitedUrls = bloomFilter;
        _urlQueue = new Queue<string>();
    }

    public async Task CrawlAsync(string startUrl)
    {
        _urlQueue.Enqueue(startUrl);

        while (_urlQueue.Count > 0)
        {
            var url = _urlQueue.Dequeue();

            // Check if already visited
            if (_visitedUrls.Contains(url))
            {
                continue; // Skip already visited URLs
            }

            // Mark as visited
            _visitedUrls.Add(url);

            // Download page
            var page = await DownloadPageAsync(url);

            // Process page
            await ProcessPageAsync(page);

            // Extract new URLs
            var newUrls = ExtractUrls(page);
            foreach (var newUrl in newUrls)
            {
                if (!_visitedUrls.Contains(newUrl))
                {
                    _urlQueue.Enqueue(newUrl);
                }
            }
        }
    }
}
```

#### 3. Distributed Deduplication (Multiple Instances)

```csharp
// Configure distributed Bloom filter
services.AddBloomFilter(setupAction =>
{
    setupAction.UseRedis(new FilterRedisOptions
    {
        Name = "GlobalDeduplication",
        RedisKey = "BF:Dedup",
        Endpoints = new List<string> { "redis-cluster:6379" },
        ExpectedElements = 100_000_000,
        ErrorRate = 0.0001
    });
});

// Use across multiple service instances
public class MessageProcessor
{
    private readonly IBloomFilter _bloomFilter;

    public async Task ProcessMessageAsync(Message message)
    {
        // All instances share the same Redis Bloom filter
        if (await _bloomFilter.ContainsAsync(message.Id))
        {
            // Message already processed by another instance
            return;
        }

        // Mark as processed
        await _bloomFilter.AddAsync(message.Id);

        // Process message
        await HandleMessageAsync(message);
    }
}
```

## Hash Algorithms

BloomFilter.NetCore supports 20+ hash algorithms, choose based on performance and accuracy requirements:

### Algorithm Categories

| Category | Algorithms | Characteristics | Use Cases |
|----------|-----------|-----------------|-----------|
| **LCG-based** | LCGWithFNV1<br>LCGWithFNV1a<br>LCGModifiedFNV1 | Extremely fast, lower quality | Extremely high performance requirements, can tolerate high false positive rates |
| **RNG-based** | RNGWithFNV1<br>RNGWithFNV1a<br>RNGModifiedFNV1 | High quality, slower | Scenarios requiring high accuracy |
| **Checksum** | CRC32<br>CRC64<br>Adler32 | Balanced performance and quality | General scenarios |
| **Murmur Family** | Murmur3<br>Murmur32BitsX86<br>Murmur128BitsX64<br>Murmur128BitsX86 | **Recommended**, good performance, high quality | Recommended for production |
| **Cryptographic** | SHA1<br>SHA256<br>SHA384<br>SHA512 | Highest quality, slowest | Scenarios requiring extreme security |
| **XXHash Family** | XXHash32<br>XXHash64<br>XXHash3<br>XXHash128 | **Fastest**, excellent quality | First choice for high performance |

### Selection Recommendations

```csharp
// Recommended: Default Murmur3 for production (balanced performance and quality)
var bf1 = FilterBuilder.Build(10_000_000, 0.01, HashMethod.Murmur3);

// High Performance: Choose XXHash3 for extreme performance requirements
var bf2 = FilterBuilder.Build(10_000_000, 0.01, HashMethod.XXHash3);

// High Precision: Choose SHA256 + lower errorRate for minimal false positive rate
var bf3 = FilterBuilder.Build(10_000_000, 0.0001, HashMethod.SHA256);

// Distributed: Recommend XXHash64 for Redis (fast and good cross-language support)
var bf4 = FilterRedisBuilder.Build(
    "localhost:6379",
    "MyFilter",
    10_000_000,
    0.01,
    HashMethod.XXHash64
);
```

## Performance Benchmarks

### Test Environment

```
BenchmarkDotNet=v0.13.5
OS: Windows 11 (10.0.22621.1778/22H2)
CPU: AMD Ryzen 7 5800X, 1 CPU, 16 logical cores, 8 physical cores
.NET SDK: 7.0.304
Runtime: .NET 7.0.7 (7.0.723.27404), X64 RyuJIT AVX2
```

### Performance Rankings (64-byte data)

| Rank | Algorithm | Mean Time | Relative Speed |
|------|-----------|-----------|----------------|
| 🥇 1 | XXHash3 | 33.14 ns | Baseline (Fastest) |
| 🥈 2 | XXHash128 | 36.01 ns | 1.09x |
| 🥉 3 | CRC64 | 38.83 ns | 1.17x |
| 4 | XXHash64 | 50.62 ns | 1.53x |
| 5 | Murmur3 | 70.98 ns | 2.14x |
| ... | ... | ... | ... |
| 28 | SHA512 | 1,368.20 ns | 41.28x (Slowest) |

### Complete Performance Data

<details>
<summary>Click to expand full benchmark results</summary>

#### 64-byte Data

| Algorithm | Mean Time | Error | StdDev | Allocated |
|-----------|-----------|-------|--------|-----------|
| XXHash3 | 33.14 ns | 0.295 ns | 0.276 ns | 80 B |
| XXHash128 | 36.01 ns | 0.673 ns | 0.749 ns | 80 B |
| CRC64 | 38.83 ns | 0.399 ns | 0.333 ns | 80 B |
| XXHash64 | 50.62 ns | 0.756 ns | 0.670 ns | 80 B |
| Murmur3 | 70.98 ns | 1.108 ns | 1.036 ns | 80 B |
| XXHash32 | 73.15 ns | 0.526 ns | 0.466 ns | 80 B |
| Murmur128BitsX64 | 80.15 ns | 0.783 ns | 0.653 ns | 120 B |
| Murmur128BitsX86 | 82.73 ns | 1.211 ns | 1.011 ns | 120 B |
| LCGWithFNV1 | 91.27 ns | 1.792 ns | 2.134 ns | 80 B |
| CRC32 | 145.63 ns | 1.528 ns | 1.429 ns | 328 B |
| Adler32 | 150.07 ns | 0.664 ns | 0.589 ns | 336 B |
| RNGWithFNV1 | 445.32 ns | 8.463 ns | 9.747 ns | 384 B |
| SHA256 | 922.30 ns | 4.478 ns | 3.739 ns | 496 B |
| SHA1 | 1,045.67 ns | 6.411 ns | 5.997 ns | 464 B |
| SHA384 | 1,173.67 ns | 5.050 ns | 3.942 ns | 456 B |
| SHA512 | 1,368.20 ns | 10.967 ns | 9.722 ns | 504 B |

#### 1 MB Data

| Algorithm | Mean Time |
|-----------|-----------|
| XXHash3 | 30,258.92 ns (~30 μs) |
| XXHash128 | 33,778.68 ns (~34 μs) |
| CRC64 | 56,321.74 ns (~56 μs) |
| XXHash64 | 100,570.79 ns (~101 μs) |
| Murmur128BitsX64 | 163,915.44 ns (~164 μs) |
| ... | ... |
| SHA1 | 3,381,425.73 ns (~3.4 ms) |

</details>

### Performance Recommendations

1. **General Scenarios**: Use `Murmur3` (default), balanced performance and quality
2. **Extreme Performance**: Use `XXHash3`, 2x faster than Murmur3
3. **Large Data**: Use `XXHash128` or `Murmur128BitsX64`, 128-bit output reduces collisions
4. **Avoid**: LCG series (poor quality), SHA series (too slow)

## Advanced Usage

### Serialization and Deserialization

```csharp
// Export Bloom filter state
var bf = FilterBuilder.Build(1_000_000, 0.01);
bf.Add("item1");
bf.Add("item2");

// Get internal state (for persistence)
var memory = (FilterMemory)bf;
var buckets = memory.Buckets; // BitArray[]
var bucketBytes = memory.BucketBytes; // byte[][]

// Restore Bloom filter from state
var options = new FilterMemoryOptions
{
    Name = "RestoredFilter",
    ExpectedElements = 1_000_000,
    ErrorRate = 0.01,
    Buckets = buckets // Or use BucketBytes
};
var restoredBf = FilterBuilder.Build(options);

Console.WriteLine(restoredBf.Contains("item1")); // True
```

### Batch Operations

```csharp
// Batch add
var items = Enumerable.Range(1, 10000)
    .Select(i => Encoding.UTF8.GetBytes($"user:{i}"))
    .ToArray();

var addResults = bf.Add(items);
Console.WriteLine($"Successfully added: {addResults.Count(r => r)} elements");

// Batch check
var checkResults = bf.Contains(items);
Console.WriteLine($"Exist: {checkResults.Count(r => r)} elements");

// Check if all elements exist
bool allExist = bf.All(items);

// Async batch operations
var asyncAddResults = await bf.AddAsync(items);
var asyncCheckResults = await bf.ContainsAsync(items);
bool asyncAllExist = await bf.AllAsync(items);
```

### Custom Hash Function

```csharp
using BloomFilter.HashAlgorithms;

// Implement custom hash algorithm
public class MyCustomHash : HashFunction
{
    public override long ComputeHash(ReadOnlySpan<byte> data)
    {
        // Custom hash logic
        long hash = 0;
        foreach (var b in data)
        {
            hash = hash * 31 + b;
        }
        return hash;
    }
}

// Use custom hash
var customHash = new MyCustomHash();
var bf = FilterBuilder.Build(1_000_000, 0.01, customHash);
```

### Calculate Actual False Positive Rate

```csharp
var bf = FilterBuilder.Build(100_000, 0.01);

// Add 50,000 elements
for (int i = 0; i < 50_000; i++)
{
    bf.Add($"item:{i}");
}

// Calculate theoretical false positive rate
var filter = (Filter)bf;
double theoreticalErrorRate = Filter.BestP(
    filter.Hashes,
    filter.Capacity,
    50_000
);

Console.WriteLine($"Theoretical error rate: {theoreticalErrorRate:P4}");

// Test actual false positive rate
int falsePositives = 0;
int testCount = 100_000;

for (int i = 50_000; i < 50_000 + testCount; i++)
{
    if (bf.Contains($"item:{i}"))
    {
        falsePositives++;
    }
}

double actualErrorRate = (double)falsePositives / testCount;
Console.WriteLine($"Actual error rate: {actualErrorRate:P4}");
Console.WriteLine($"False positives: {falsePositives} / {testCount}");
```

### Monitoring and Statistics

```csharp
public class BloomFilterMonitor
{
    private readonly IBloomFilter _filter;
    private long _addCount;
    private long _hitCount;
    private long _missCount;

    public BloomFilterMonitor(IBloomFilter filter)
    {
        _filter = filter;
    }

    public bool Add(string item)
    {
        Interlocked.Increment(ref _addCount);
        return _filter.Add(item);
    }

    public bool Contains(string item)
    {
        var result = _filter.Contains(item);
        if (result)
            Interlocked.Increment(ref _hitCount);
        else
            Interlocked.Increment(ref _missCount);
        return result;
    }

    public void PrintStats()
    {
        Console.WriteLine($"Total adds: {_addCount}");
        Console.WriteLine($"Hits: {_hitCount}");
        Console.WriteLine($"Misses: {_missCount}");
        Console.WriteLine($"Hit rate: {(double)_hitCount / (_hitCount + _missCount):P2}");
    }
}
```

## API Reference

### IBloomFilter Interface

```csharp
public interface IBloomFilter : IDisposable
{
    // Properties
    string Name { get; }

    // Synchronous methods
    bool Add(ReadOnlySpan<byte> data);
    IList<bool> Add(IEnumerable<byte[]> elements);
    bool Contains(ReadOnlySpan<byte> element);
    IList<bool> Contains(IEnumerable<byte[]> elements);
    bool All(IEnumerable<byte[]> elements);
    void Clear();
    long[] ComputeHash(ReadOnlySpan<byte> data);

    // Asynchronous methods
    ValueTask<bool> AddAsync(ReadOnlyMemory<byte> data);
    ValueTask<IList<bool>> AddAsync(IEnumerable<byte[]> elements);
    ValueTask<bool> ContainsAsync(ReadOnlyMemory<byte> element);
    ValueTask<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements);
    ValueTask<bool> AllAsync(IEnumerable<byte[]> elements);
    ValueTask ClearAsync();
}
```

### Filter Base Class

```csharp
public abstract class Filter : IBloomFilter
{
    // Properties
    public string Name { get; }
    public HashFunction Hash { get; }
    public long Capacity { get; }
    public int Hashes { get; }
    public long ExpectedElements { get; }
    public double ErrorRate { get; }

    // Static methods (mathematical calculations)
    public static long BestM(long n, double p);
    public static int BestK(long n, long m);
    public static long BestN(int k, long m);
    public static double BestP(int k, long m, long insertedElements);
}
```

### FilterBuilder

```csharp
public static class FilterBuilder
{
    // Using expected elements and error rate
    public static IBloomFilter Build(long expectedElements, double errorRate);
    public static IBloomFilter Build(long expectedElements, double errorRate, HashMethod method);
    public static IBloomFilter Build(long expectedElements, double errorRate, HashFunction hash);

    // Using capacity and number of hash functions
    public static IBloomFilter Build(long capacity, int hashes, HashMethod method);
    public static IBloomFilter Build(long capacity, int hashes, HashFunction hash);

    // Using configuration object
    public static IBloomFilter Build(FilterMemoryOptions options);
}
```

### FilterRedisBuilder

```csharp
public static class FilterRedisBuilder
{
    public static IBloomFilter Build(
        string redisHost,
        string name,
        long expectedElements,
        double errorRate,
        HashMethod method = HashMethod.Murmur3);
}
```

### Extension Methods

```csharp
// Service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBloomFilter(
        this IServiceCollection services,
        Action<BloomFilterOptions> setupAction);
}

// Configuration extensions
public static class BloomFilterOptionsExtensions
{
    public static BloomFilterOptions UseInMemory(
        this BloomFilterOptions options,
        Action<FilterMemoryOptions> setup = null);

    public static BloomFilterOptions UseRedis(
        this BloomFilterOptions options,
        FilterRedisOptions setup);

    public static BloomFilterOptions UseCSRedis(
        this BloomFilterOptions options,
        FilterCSRedisOptions setup);

    public static BloomFilterOptions UseFreeRedis(
        this BloomFilterOptions options,
        FilterFreeRedisOptions setup);

    public static BloomFilterOptions UseEasyCachingRedis(
        this BloomFilterOptions options,
        FilterEasyCachingRedisOptions setup);
}
```

## Frequently Asked Questions (FAQ)

### 1. What is the false positive rate of a Bloom filter?

The false positive rate is determined by the `errorRate` parameter you specify when creating the filter. For example:

```csharp
// 1% false positive rate
var bf = FilterBuilder.Build(1_000_000, 0.01);

// 0.1% false positive rate (more accurate, but uses more memory)
var bf2 = FilterBuilder.Build(1_000_000, 0.001);
```

**Note**: Lower error rates require more memory space.

### 2. How to choose expectedElements?

`expectedElements` should be set to the number of elements you expect to add. If the actual number exceeds this, the false positive rate will increase.

Recommendations:
- Estimate actual element count
- Add 20%-50% buffer
- Monitor actual false positive rate regularly

### 3. In-Memory vs Redis Mode - How to Choose?

| Scenario | Recommended Mode | Reason |
|----------|-----------------|---------|
| Single-instance application | In-Memory | Highest performance, no network overhead |
| Multi-instance application | Redis | Shared state, distributed support |
| Persistence required | Redis | Redis provides persistence |
| Temporary deduplication | In-Memory | Simple and fast |
| Cross-service sharing | Redis | Multi-language access support |

### 4. How to clear a Bloom filter?

```csharp
// Synchronous clear
bf.Clear();

// Asynchronous clear
await bf.ClearAsync();
```

**Note**: Clear operation deletes all data, use with caution!

### 5. How much memory does a Bloom filter use?

Memory usage depends on capacity (m):

```
Memory (bytes) = m / 8
```

Example calculation:

```csharp
// 10 million elements, 1% false positive rate
var bf = FilterBuilder.Build(10_000_000, 0.01);
var filter = (Filter)bf;

// Calculate memory usage
long bits = filter.Capacity;
long bytes = bits / 8;
double mb = bytes / (1024.0 * 1024.0);

Console.WriteLine($"Bit array size: {bits:N0} bits");
Console.WriteLine($"Memory usage: {bytes:N0} bytes ({mb:F2} MB)");
// Output: approximately 11.4 MB
```

### 6. Can elements be deleted?

**No**. Standard Bloom filters do not support deletion because:
- Multiple elements may map to the same bits
- Deleting one element may affect detection of other elements

If deletion is needed, consider:
- Counting Bloom Filter
- Cuckoo Filter

### 7. Is it thread-safe?

Yes, BloomFilter.NetCore is thread-safe:

```csharp
// Multi-threaded concurrent access
var bf = FilterBuilder.Build(10_000_000, 0.01);

Parallel.For(0, 1000, i =>
{
    bf.Add($"item:{i}"); // Thread-safe
});

Parallel.For(0, 1000, i =>
{
    var exists = bf.Contains($"item:{i}"); // Thread-safe
});
```

### 8. How to monitor Redis connections?

```csharp
// Use StackExchange.Redis connection monitoring
services.AddBloomFilter(setupAction =>
{
    setupAction.UseRedis(new FilterRedisOptions
    {
        Name = "MyFilter",
        RedisKey = "BF:Key",
        Endpoints = new List<string> { "localhost:6379" },
        // Enable connection logging
        AbortOnConnectFail = false,
        ConnectTimeout = 5000,
        ConnectRetry = 3
    });
});

// Get Redis connection information
var bf = serviceProvider.GetService<IBloomFilter>();
if (bf is FilterRedis redisFilter)
{
    var connection = redisFilter.Connection;
    Console.WriteLine($"Connection status: {connection.IsConnected}");
    Console.WriteLine($"Endpoints: {string.Join(", ", connection.GetEndPoints())}");
}
```

## Contributing

We welcome community contributions!

### How to Contribute

1. Fork this repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Create a Pull Request

### Development Guidelines

```bash
# Clone repository
git clone https://github.com/vla/BloomFilter.NetCore.git
cd BloomFilter.NetCore

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run tests
dotnet test

# Run benchmarks
cd test/BenchmarkTest
dotnet run -c Release
```

### Code Standards

- Follow C# coding conventions
- Add XML documentation comments
- Write unit tests
- Update relevant documentation

## Acknowledgments

Thanks to all developers who contributed to this project!

Special thanks to:
- .NET Foundation
- StackExchange.Redis team
- All dependency library authors

If this project helps you, please give us a ⭐️ Star!
