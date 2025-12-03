# BloomFilter.NetCore

[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6.0%20|%207.0%20|%208.0%20|%209.0%20|%2010.0-blue.svg)](https://dotnet.microsoft.com/)

一个高性能、功能完整的 .NET 布隆过滤器实现库，支持内存存储和多种 Redis 分布式后端。

## 目录

- [项目概述](#项目概述)
- [主要特性](#主要特性)
- [包和状态](#包和状态)
- [整体架构](#整体架构)
- [核心功能](#核心功能)
- [安装](#安装)
- [快速开始](#快速开始)
- [使用示例](#使用示例)
  - [内存模式](#内存模式)
  - [依赖注入配置](#依赖注入配置)
  - [Redis 分布式模式](#redis-分布式模式)
- [哈希算法](#哈希算法)
- [性能基准测试](#性能基准测试)
- [高级用法](#高级用法)
- [API 参考](#api-参考)
- [贡献指南](#贡献指南)
- [许可证](#许可证)

## 项目概述

BloomFilter.NetCore 是一个企业级的布隆过滤器库，专为 .NET 生态系统设计。布隆过滤器是一种空间效率极高的概率型数据结构,用于测试一个元素是否属于一个集合。它的核心特点是:

- **空间高效**: 相比传统的 HashSet,占用空间极小
- **时间复杂度 O(1)**: 添加和查询操作都是常数时间
- **允许一定误报率**: 可能返回假阳性(false positive),但绝不会出现假阴性(false negative)

本项目提供了两大类实现:

1. **内存布隆过滤器 (FilterMemory)**: 基于 BitArray 的内存实现,适用于单进程场景
2. **分布式布隆过滤器 (FilterRedis 系列)**: 基于 Redis 的分布式实现,支持多应用程序并发访问

### 主要用途

- **缓存穿透防护**: 防止恶意查询不存在的数据导致缓存击穿
- **去重场景**: URL 去重、邮箱去重、用户 ID 去重等
- **推荐系统**: 判断用户是否已看过某个内容
- **爬虫系统**: 判断 URL 是否已被爬取
- **分布式系统**: 多服务实例间共享状态判断
- **大数据场景**: 海量数据的存在性判断

## 主要特性

### 🎯 灵活配置

- **参数完全可配置**: 位数组大小 (m)、哈希函数数量 (k)
- **自动参数计算**: 根据容忍的误报率 (p) 和预期元素数量 (n) 自动计算最优参数
- **20+ 种哈希算法**: 支持 CRC、MD5、SHA、Murmur、LCGs、xxHash 等或自定义算法

### ⚡ 高性能

- **快速生成**: 布隆过滤器的生成和操作都极快
- **优化实现**: 使用 Span<T>、ReadOnlyMemory<T> 等零拷贝技术
- **不安全代码优化**: 在性能关键路径使用 unsafe 代码块
- **拒绝采样**: 实现了拒绝采样和哈希链,考虑雪崩效应以提高哈希质量

### 🔒 并发安全

- **线程安全**: 使用 AsyncLock 机制确保多线程并发访问安全
- **异步支持**: 全面的 async/await 支持,所有操作都有异步版本
- **分布式锁**: Redis 实现支持跨应用程序的并发访问

### 🌐 多后端支持

- **StackExchange.Redis**: 官方推荐的 Redis 客户端
- **CSRedisCore**: 高性能的 Redis 客户端
- **FreeRedis**: 轻量级 Redis 客户端
- **EasyCaching**: 支持 EasyCaching 抽象层,可切换多种缓存提供程序

### 📦 现代 .NET 支持

- **多框架支持**: net462, netstandard2.0, net6.0, net7.0, net8.0, net9.0, net10.0
- **依赖注入**: 原生支持 Microsoft.Extensions.DependencyInjection
- **可空引用类型**: 启用可空引用类型,提高代码安全性

## 包和状态

| 包名 | NuGet | 说明 |
|------|-------|------|
|**BloomFilter.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.NetCore)| 核心包,提供内存布隆过滤器 |
|**BloomFilter.Redis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.Redis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.Redis.NetCore)| StackExchange.Redis 实现 |
|**BloomFilter.CSRedis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.CSRedis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.CSRedis.NetCore)| CSRedisCore 实现 |
|**BloomFilter.FreeRedis.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.FreeRedis.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.FreeRedis.NetCore)| FreeRedis 实现 |
|**BloomFilter.EasyCaching.NetCore**|[![nuget](https://img.shields.io/nuget/v/BloomFilter.EasyCaching.NetCore.svg?style=flat-square)](https://www.nuget.org/packages/BloomFilter.EasyCaching.NetCore)| EasyCaching 集成 |

## 整体架构

### 核心接口层

```
IBloomFilter (接口)
    ├── Add / AddAsync           - 添加元素
    ├── Contains / ContainsAsync - 检查元素
    ├── All / AllAsync           - 批量检查
    ├── Clear / ClearAsync       - 清空过滤器
    └── ComputeHash              - 计算哈希值
```

### 实现层次结构

```
Filter (抽象基类)
    ├── FilterMemory (内存实现)
    │   └── 使用 BitArray 存储
    │
    └── Redis 系列 (分布式实现)
        ├── FilterRedis (StackExchange.Redis)
        ├── FilterCSRedis (CSRedisCore)
        ├── FilterFreeRedis (FreeRedis)
        └── FilterEasyCachingRedis (EasyCaching)
```

### 配置系统

```
BloomFilterOptions
    ├── FilterMemoryOptions      - 内存模式配置
    ├── FilterRedisOptions       - StackExchange.Redis 配置
    ├── FilterCSRedisOptions     - CSRedisCore 配置
    ├── FilterFreeRedisOptions   - FreeRedis 配置
    └── FilterEasyCachingOptions - EasyCaching 配置
```

## 核心功能

### 数学模型

BloomFilter.NetCore 实现了完整的布隆过滤器数学模型:

#### 1. 最优位数组大小 (m)

给定预期元素数 `n` 和误报率 `p`,计算最优的位数组大小:

```
m = -(n * ln(p)) / (ln(2)^2)
```

#### 2. 最优哈希函数数量 (k)

给定元素数 `n` 和位数组大小 `m`,计算最优的哈希函数数量:

```
k = (m / n) * ln(2)
```

#### 3. 实际误报率 (p)

给定已插入元素数、哈希函数数量和位数组大小,计算实际误报率:

```
p = (1 - e^(-k*n/m))^k
```

这些计算由 `Filter` 基类提供的静态方法实现:

```csharp
// 计算最优位数组大小
long m = Filter.BestM(expectedElements, errorRate);

// 计算最优哈希函数数量
int k = Filter.BestK(expectedElements, capacity);

// 计算最优元素数量
long n = Filter.BestN(hashes, capacity);

// 计算实际误报率
double p = Filter.BestP(hashes, capacity, insertedElements);
```

### 存储机制

#### 内存存储

- **BitArray**: 使用 .NET 的 BitArray 作为底层存储
- **分桶策略**: 当容量超过 2GB (MaxInt = 2,147,483,640) 时,自动分成多个 BitArray
- **序列化支持**: 支持序列化/反序列化以持久化或传输过滤器状态

#### Redis 存储

- **SETBIT/GETBIT**: 使用 Redis 的位操作命令
- **分布式访问**: 多个应用实例可以并发访问同一个过滤器
- **持久化**: 利用 Redis 的持久化机制保证数据安全

### 并发控制

```csharp
// AsyncLock 确保线程安全
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

## 安装

### 通过 NuGet 安装

**内存模式 (核心包):**

```bash
dotnet add package BloomFilter.NetCore
```

**Redis 分布式模式 (选择一个):**

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

## 快速开始

### 最简单的示例

```csharp
using BloomFilter;

// 创建一个布隆过滤器:预期 1000 万元素,1% 误报率
var bf = FilterBuilder.Build(10_000_000, 0.01);

// 添加元素
bf.Add("user:123");
bf.Add("user:456");

// 检查元素是否存在
Console.WriteLine(bf.Contains("user:123")); // True
Console.WriteLine(bf.Contains("user:789")); // False (可能极小概率为 True)

// 清空过滤器
bf.Clear();
```

### 异步操作

```csharp
// 异步添加
await bf.AddAsync(Encoding.UTF8.GetBytes("user:123"));

// 异步检查
bool exists = await bf.ContainsAsync(Encoding.UTF8.GetBytes("user:123"));

// 批量异步操作
var users = new[] {
    Encoding.UTF8.GetBytes("user:1"),
    Encoding.UTF8.GetBytes("user:2"),
    Encoding.UTF8.GetBytes("user:3")
};

await bf.AddAsync(users);
var results = await bf.ContainsAsync(users);
```

## 使用示例

### 内存模式

#### 基本用法

```csharp
using BloomFilter;

public class UserService
{
    // 静态共享的布隆过滤器
    private static readonly IBloomFilter _bloomFilter =
        FilterBuilder.Build(10_000_000, 0.01);

    public void AddUser(string userId)
    {
        // 添加用户 ID
        _bloomFilter.Add(userId);
    }

    public bool MayExistUser(string userId)
    {
        // 检查用户是否可能存在
        return _bloomFilter.Contains(userId);
    }
}
```

#### 自定义配置

```csharp
using BloomFilter;

// 方式 1: 指定哈希算法
var bf1 = FilterBuilder.Build(
    expectedElements: 1_000_000,
    errorRate: 0.001,
    hashMethod: HashMethod.Murmur3
);

// 方式 2: 使用自定义哈希函数
var hashFunction = new Murmur128BitsX64();
var bf2 = FilterBuilder.Build(
    expectedElements: 1_000_000,
    errorRate: 0.001,
    hashFunction: hashFunction
);

// 方式 3: 手动指定参数 (高级用法)
var bf3 = FilterBuilder.Build(
    capacity: 9585059,      // 位数组大小
    hashes: 10,             // 哈希函数数量
    hashMethod: HashMethod.XXHash3
);

// 方式 4: 使用配置对象
var options = new FilterMemoryOptions
{
    Name = "MyFilter",
    ExpectedElements = 5_000_000,
    ErrorRate = 0.01,
    Method = HashMethod.Murmur3
};
var bf4 = FilterBuilder.Build(options);
```

### 依赖注入配置

#### ASP.NET Core 集成

```csharp
using BloomFilter;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 注册布隆过滤器服务
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

// 在控制器或服务中使用
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
            // 用户可能存在,继续查询数据库
            return Ok("用户可能存在");
        }
        else
        {
            // 用户一定不存在,无需查询数据库
            return NotFound("用户不存在");
        }
    }
}
```

#### 多个过滤器实例

```csharp
services.AddBloomFilter(setupAction =>
{
    // 用户过滤器
    setupAction.UseInMemory(options =>
    {
        options.Name = "UserFilter";
        options.ExpectedElements = 10_000_000;
        options.ErrorRate = 0.01;
    });

    // 邮箱过滤器
    setupAction.UseInMemory(options =>
    {
        options.Name = "EmailFilter";
        options.ExpectedElements = 5_000_000;
        options.ErrorRate = 0.001;
    });
});

// 使用工厂获取指定过滤器
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

### Redis 分布式模式

#### StackExchange.Redis

```csharp
using BloomFilter;

// 方式 1: 直接构建
var bf = FilterRedisBuilder.Build(
    redisHost: "localhost:6379",
    name: "DistributedFilter",
    expectedElements: 5_000_000,
    errorRate: 0.001
);

bf.Add("item:123");
Console.WriteLine(bf.Contains("item:123")); // True

// 方式 2: 依赖注入
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

// 方式 3: 高级配置 (主从、哨兵、集群)
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

#### EasyCaching 集成

EasyCaching 提供了统一的缓存抽象层,允许您轻松切换底层缓存实现:

```csharp
using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// 1. 配置 EasyCaching
services.AddEasyCaching(options =>
{
    // 配置 Redis 提供程序
    options.UseRedis(config =>
    {
        config.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
        config.DBConfig.Database = 0;
    }, "redis-provider-1");

    // 可以配置多个提供程序
    options.UseRedis(config =>
    {
        config.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
        config.DBConfig.Database = 1;
    }, "redis-provider-2");
});

// 2. 配置 BloomFilter
services.AddBloomFilter(setupAction =>
{
    // 使用第一个 Redis 提供程序
    setupAction.UseEasyCachingRedis(new FilterEasyCachingRedisOptions
    {
        Name = "BF1",
        RedisKey = "BloomFilter1",
        ProviderName = "redis-provider-1",
        ExpectedElements = 10_000_000,
        ErrorRate = 0.01
    });

    // 使用第二个 Redis 提供程序
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

// 使用默认过滤器
var bf = provider.GetService<IBloomFilter>();
bf.Add("value1");

// 使用指定名称的过滤器
var factory = provider.GetService<IBloomFilterFactory>();
var bf1 = factory.Get("BF1");
var bf2 = factory.Get("BF2");

bf1.Add("item1");
bf2.Add("item2");
```

### 实际应用场景

#### 1. 防止缓存穿透

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
        // 第一层防护: 布隆过滤器
        if (!_bloomFilter.Contains(productId))
        {
            // 商品一定不存在,直接返回 null
            return null;
        }

        // 第二层: 缓存
        var cached = await _cache.GetAsync<Product>(productId);
        if (cached != null)
        {
            return cached;
        }

        // 第三层: 数据库
        var product = await _repository.GetByIdAsync(productId);
        if (product != null)
        {
            await _cache.SetAsync(productId, product);
        }

        return product;
    }

    public async Task CreateProductAsync(Product product)
    {
        // 保存到数据库
        await _repository.SaveAsync(product);

        // 添加到布隆过滤器
        _bloomFilter.Add(product.Id);

        // 更新缓存
        await _cache.SetAsync(product.Id, product);
    }
}
```

#### 2. URL 去重 (爬虫系统)

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

            // 检查是否已访问
            if (_visitedUrls.Contains(url))
            {
                continue; // 跳过已访问的 URL
            }

            // 标记为已访问
            _visitedUrls.Add(url);

            // 抓取页面
            var page = await DownloadPageAsync(url);

            // 处理页面
            await ProcessPageAsync(page);

            // 提取新的 URL
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

#### 3. 分布式去重 (多实例)

```csharp
// 配置分布式布隆过滤器
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

// 在多个服务实例中使用
public class MessageProcessor
{
    private readonly IBloomFilter _bloomFilter;

    public async Task ProcessMessageAsync(Message message)
    {
        // 所有实例共享同一个 Redis 布隆过滤器
        if (await _bloomFilter.ContainsAsync(message.Id))
        {
            // 消息已被其他实例处理
            return;
        }

        // 标记为已处理
        await _bloomFilter.AddAsync(message.Id);

        // 处理消息
        await HandleMessageAsync(message);
    }
}
```

## 哈希算法

BloomFilter.NetCore 支持 20+ 种哈希算法,可根据性能和准确性需求选择:

### 算法分类

| 类别 | 算法 | 特点 | 适用场景 |
|------|------|------|----------|
| **LCG 类** | LCGWithFNV1<br>LCGWithFNV1a<br>LCGModifiedFNV1 | 极快,但质量较低 | 性能要求极高,可容忍高误报率 |
| **RNG 类** | RNGWithFNV1<br>RNGWithFNV1a<br>RNGModifiedFNV1 | 质量高,但较慢 | 对准确性要求高的场景 |
| **校验和** | CRC32<br>CRC64<br>Adler32 | 平衡性能和质量 | 通用场景 |
| **Murmur 系列** | Murmur3<br>Murmur32BitsX86<br>Murmur128BitsX64<br>Murmur128BitsX86 | **推荐**,性能好,质量高 | 生产环境推荐 |
| **加密哈希** | SHA1<br>SHA256<br>SHA384<br>SHA512 | 质量最高,但最慢 | 安全性要求极高的场景 |
| **XXHash 系列** | XXHash32<br>XXHash64<br>XXHash3<br>XXHash128 | **最快**,质量优秀 | 高性能场景首选 |

### 选择建议

```csharp
// 推荐: 生产环境默认选择 Murmur3 (平衡性能和质量)
var bf1 = FilterBuilder.Build(10_000_000, 0.01, HashMethod.Murmur3);

// 高性能: 对性能要求极高,选择 XXHash3
var bf2 = FilterBuilder.Build(10_000_000, 0.01, HashMethod.XXHash3);

// 高精度: 对误报率要求极低,选择 SHA256 + 更低的 errorRate
var bf3 = FilterBuilder.Build(10_000_000, 0.0001, HashMethod.SHA256);

// 分布式: Redis 场景推荐 XXHash64 (速度快且跨语言支持好)
var bf4 = FilterRedisBuilder.Build(
    "localhost:6379",
    "MyFilter",
    10_000_000,
    0.01,
    HashMethod.XXHash64
);
```

## 性能基准测试

### 测试环境

```
BenchmarkDotNet=v0.13.5
OS: Windows 11 (10.0.22621.1778/22H2)
CPU: AMD Ryzen 7 5800X, 1 CPU, 16 logical cores, 8 physical cores
.NET SDK: 7.0.304
Runtime: .NET 7.0.7 (7.0.723.27404), X64 RyuJIT AVX2
```

### 性能排名 (64 字节数据)

| 排名 | 算法 | 平均时间 | 相对速度 |
|------|------|----------|----------|
| 🥇 1 | XXHash3 | 33.14 ns | 基准 (最快) |
| 🥈 2 | XXHash128 | 36.01 ns | 1.09x |
| 🥉 3 | CRC64 | 38.83 ns | 1.17x |
| 4 | XXHash64 | 50.62 ns | 1.53x |
| 5 | Murmur3 | 70.98 ns | 2.14x |
| ... | ... | ... | ... |
| 28 | SHA512 | 1,368.20 ns | 41.28x (最慢) |

### 完整性能数据

<details>
<summary>点击展开完整基准测试结果</summary>

#### 64 字节数据

| 算法 | 平均时间 | 误差 | 标准差 | 内存分配 |
|------|---------|------|--------|---------|
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

#### 1 MB 数据

| 算法 | 平均时间 |
|------|---------|
| XXHash3 | 30,258.92 ns (~30 μs) |
| XXHash128 | 33,778.68 ns (~34 μs) |
| CRC64 | 56,321.74 ns (~56 μs) |
| XXHash64 | 100,570.79 ns (~101 μs) |
| Murmur128BitsX64 | 163,915.44 ns (~164 μs) |
| ... | ... |
| SHA1 | 3,381,425.73 ns (~3.4 ms) |

</details>

### 性能建议

1. **通用场景**: 使用 `Murmur3` (默认),性能和质量平衡
2. **极限性能**: 使用 `XXHash3`,比 Murmur3 快 2 倍
3. **大数据**: 使用 `XXHash128` 或 `Murmur128BitsX64`,128 位输出减少碰撞
4. **避免使用**: LCG 系列 (质量差)、SHA 系列 (太慢)

## 高级用法

### 序列化和反序列化

```csharp
// 导出布隆过滤器状态
var bf = FilterBuilder.Build(1_000_000, 0.01);
bf.Add("item1");
bf.Add("item2");

// 获取内部状态 (用于持久化)
var memory = (FilterMemory)bf;
var buckets = memory.Buckets; // BitArray[]
var bucketBytes = memory.BucketBytes; // byte[][]

// 从状态恢复布隆过滤器
var options = new FilterMemoryOptions
{
    Name = "RestoredFilter",
    ExpectedElements = 1_000_000,
    ErrorRate = 0.01,
    Buckets = buckets // 或使用 BucketBytes
};
var restoredBf = FilterBuilder.Build(options);

Console.WriteLine(restoredBf.Contains("item1")); // True
```

### 批量操作

```csharp
// 批量添加
var items = Enumerable.Range(1, 10000)
    .Select(i => Encoding.UTF8.GetBytes($"user:{i}"))
    .ToArray();

var addResults = bf.Add(items);
Console.WriteLine($"成功添加: {addResults.Count(r => r)} 个元素");

// 批量检查
var checkResults = bf.Contains(items);
Console.WriteLine($"存在: {checkResults.Count(r => r)} 个元素");

// 检查所有元素是否都存在
bool allExist = bf.All(items);

// 异步批量操作
var asyncAddResults = await bf.AddAsync(items);
var asyncCheckResults = await bf.ContainsAsync(items);
bool asyncAllExist = await bf.AllAsync(items);
```

### 自定义哈希函数

```csharp
using BloomFilter.HashAlgorithms;

// 实现自定义哈希算法
public class MyCustomHash : HashFunction
{
    public override long ComputeHash(ReadOnlySpan<byte> data)
    {
        // 自定义哈希逻辑
        long hash = 0;
        foreach (var b in data)
        {
            hash = hash * 31 + b;
        }
        return hash;
    }
}

// 使用自定义哈希
var customHash = new MyCustomHash();
var bf = FilterBuilder.Build(1_000_000, 0.01, customHash);
```

### 计算实际误报率

```csharp
var bf = FilterBuilder.Build(100_000, 0.01);

// 添加 50,000 个元素
for (int i = 0; i < 50_000; i++)
{
    bf.Add($"item:{i}");
}

// 计算理论误报率
var filter = (Filter)bf;
double theoreticalErrorRate = Filter.BestP(
    filter.Hashes,
    filter.Capacity,
    50_000
);

Console.WriteLine($"理论误报率: {theoreticalErrorRate:P4}");

// 测试实际误报率
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
Console.WriteLine($"实际误报率: {actualErrorRate:P4}");
Console.WriteLine($"误报数量: {falsePositives} / {testCount}");
```

### 监控和统计

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
        Console.WriteLine($"总添加: {_addCount}");
        Console.WriteLine($"命中: {_hitCount}");
        Console.WriteLine($"未命中: {_missCount}");
        Console.WriteLine($"命中率: {(double)_hitCount / (_hitCount + _missCount):P2}");
    }
}
```

## API 参考

### IBloomFilter 接口

```csharp
public interface IBloomFilter : IDisposable
{
    // 属性
    string Name { get; }

    // 同步方法
    bool Add(ReadOnlySpan<byte> data);
    IList<bool> Add(IEnumerable<byte[]> elements);
    bool Contains(ReadOnlySpan<byte> element);
    IList<bool> Contains(IEnumerable<byte[]> elements);
    bool All(IEnumerable<byte[]> elements);
    void Clear();
    long[] ComputeHash(ReadOnlySpan<byte> data);

    // 异步方法
    ValueTask<bool> AddAsync(ReadOnlyMemory<byte> data);
    ValueTask<IList<bool>> AddAsync(IEnumerable<byte[]> elements);
    ValueTask<bool> ContainsAsync(ReadOnlyMemory<byte> element);
    ValueTask<IList<bool>> ContainsAsync(IEnumerable<byte[]> elements);
    ValueTask<bool> AllAsync(IEnumerable<byte[]> elements);
    ValueTask ClearAsync();
}
```

### Filter 基类

```csharp
public abstract class Filter : IBloomFilter
{
    // 属性
    public string Name { get; }
    public HashFunction Hash { get; }
    public long Capacity { get; }
    public int Hashes { get; }
    public long ExpectedElements { get; }
    public double ErrorRate { get; }

    // 静态方法 (数学计算)
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
    // 使用预期元素数和误报率
    public static IBloomFilter Build(long expectedElements, double errorRate);
    public static IBloomFilter Build(long expectedElements, double errorRate, HashMethod method);
    public static IBloomFilter Build(long expectedElements, double errorRate, HashFunction hash);

    // 使用容量和哈希函数数量
    public static IBloomFilter Build(long capacity, int hashes, HashMethod method);
    public static IBloomFilter Build(long capacity, int hashes, HashFunction hash);

    // 使用配置对象
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

### 扩展方法

```csharp
// 服务注册
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBloomFilter(
        this IServiceCollection services,
        Action<BloomFilterOptions> setupAction);
}

// 配置扩展
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

## 常见问题 (FAQ)

### 1. 布隆过滤器的误报率是多少?

误报率由您在创建时指定的 `errorRate` 参数决定。例如:

```csharp
// 1% 误报率
var bf = FilterBuilder.Build(1_000_000, 0.01);

// 0.1% 误报率 (更准确,但占用更多内存)
var bf2 = FilterBuilder.Build(1_000_000, 0.001);
```

**注意**: 误报率越低,需要的内存空间越大。

### 2. 如何选择 expectedElements?

`expectedElements` 应设置为您预期要添加的元素数量。如果实际添加的元素超过这个数量,误报率会增加。

建议:
- 估算实际元素数量
- 留出 20%-50% 的冗余
- 定期监控实际误报率

### 3. 内存模式 vs Redis 模式如何选择?

| 场景 | 推荐模式 | 原因 |
|------|---------|------|
| 单实例应用 | 内存模式 | 性能最高,无网络开销 |
| 多实例应用 | Redis 模式 | 共享状态,支持分布式 |
| 需要持久化 | Redis 模式 | Redis 提供持久化 |
| 临时去重 | 内存模式 | 简单快速 |
| 跨服务共享 | Redis 模式 | 支持多语言访问 |

### 4. 如何清空布隆过滤器?

```csharp
// 同步清空
bf.Clear();

// 异步清空
await bf.ClearAsync();
```

**注意**: 清空操作会删除所有数据,谨慎使用!

### 5. 布隆过滤器占用多少内存?

内存占用取决于容量 (m):

```
内存 (字节) = m / 8
```

示例计算:

```csharp
// 1000 万元素, 1% 误报率
var bf = FilterBuilder.Build(10_000_000, 0.01);
var filter = (Filter)bf;

// 计算内存占用
long bits = filter.Capacity;
long bytes = bits / 8;
double mb = bytes / (1024.0 * 1024.0);

Console.WriteLine($"位数组大小: {bits:N0} bits");
Console.WriteLine($"内存占用: {bytes:N0} bytes ({mb:F2} MB)");
// 输出: 约 11.4 MB
```

### 6. 可以删除元素吗?

**不可以**。标准布隆过滤器不支持删除操作,因为:
- 多个元素可能映射到相同的位
- 删除一个元素可能影响其他元素的检测

如果需要删除功能,考虑使用:
- Counting Bloom Filter (计数布隆过滤器)
- Cuckoo Filter (布谷鸟过滤器)

### 7. 线程安全吗?

是的,BloomFilter.NetCore 是线程安全的:

```csharp
// 多线程并发访问
var bf = FilterBuilder.Build(10_000_000, 0.01);

Parallel.For(0, 1000, i =>
{
    bf.Add($"item:{i}"); // 线程安全
});

Parallel.For(0, 1000, i =>
{
    var exists = bf.Contains($"item:{i}"); // 线程安全
});
```

### 8. 如何监控 Redis 连接?

```csharp
// 使用 StackExchange.Redis 的连接监控
services.AddBloomFilter(setupAction =>
{
    setupAction.UseRedis(new FilterRedisOptions
    {
        Name = "MyFilter",
        RedisKey = "BF:Key",
        Endpoints = new List<string> { "localhost:6379" },
        // 启用连接日志
        AbortOnConnectFail = false,
        ConnectTimeout = 5000,
        ConnectRetry = 3
    });
});

// 获取 Redis 连接信息
var bf = serviceProvider.GetService<IBloomFilter>();
if (bf is FilterRedis redisFilter)
{
    var connection = redisFilter.Connection;
    Console.WriteLine($"连接状态: {connection.IsConnected}");
    Console.WriteLine($"端点: {string.Join(", ", connection.GetEndPoints())}");
}
```

## 贡献指南

我们欢迎社区贡献!

### 如何贡献

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建 Pull Request

### 开发指南

```bash
# 克隆仓库
git clone https://github.com/vla/BloomFilter.NetCore.git
cd BloomFilter.NetCore

# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行测试
dotnet test

# 运行基准测试
cd test/BenchmarkTest
dotnet run -c Release
```

### 代码规范

- 遵循 C# 编码规范
- 添加 XML 文档注释
- 编写单元测试
- 更新相关文档

## 许可证

本项目采用 [MIT License](LICENSE) 许可证。

```
MIT License

Copyright (c) 2018-2025 v.la@live.cn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## 致谢

感谢所有为本项目做出贡献的开发者!

特别感谢:
- .NET Foundation
- StackExchange.Redis 团队
- 所有依赖库的作者

## 联系方式

- **作者**: v.la@live.cn
- **GitHub**: [github.com/vla/BloomFilter.NetCore](https://github.com/vla/BloomFilter.NetCore)
- **问题反馈**: [GitHub Issues](https://github.com/vla/BloomFilter.NetCore/issues)

---

如果这个项目对您有帮助,请给我们一个 ⭐️ Star!
