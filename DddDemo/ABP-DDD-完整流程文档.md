# ABP 框架 + 领域驱动设计 (DDD) + .NET 10 + PostgreSQL 完整流程文档

## 项目概述

本项目是一个基于 ABP 框架 v10.4.0、.NET 10 和 PostgreSQL 的领域驱动设计 (DDD) 示例应用程序。项目实现了订单管理模块，展示了 DDD 的核心概念和实践。

---

## 目录

1. [环境准备](#1-环境准备)
2. [安装 .NET 10 SDK](#2-安装-net-10-sdk)
3. [安装 ABP CLI](#3-安装-abp-cli)
4. [创建 ABP 解决方案](#4-创建-abp-解决方案)
5. [配置 PostgreSQL 数据库](#5-配置-postgresql-数据库)
6. [领域层设计 (Domain Layer)](#6-领域层设计-domain-layer)
7. [应用层设计 (Application Layer)](#7-应用层设计-application-layer)
8. [基础设施层设计 (Infrastructure Layer)](#8-基础设施层设计-infrastructure-layer)
9. [API 层设计 (HttpApi Layer)](#9-api-层设计-httpapi-layer)
10. [数据库迁移](#10-数据库迁移)
11. [运行应用程序](#11-运行应用程序)
12. [项目结构](#12-项目结构)
13. [API 接口文档](#13-api-接口文档)

---

## 1. 环境准备

### 系统要求
- **操作系统**: Linux (Ubuntu 22.04+), Windows 10/11, macOS
- **.NET SDK**: 10.0 或更高版本
- **数据库**: PostgreSQL 14+
- **Docker**: (可选，用于运行 PostgreSQL)

### 检查现有环境
```bash
# 检查 .NET 版本
dotnet --version

# 检查 Docker
docker --version
```

---

## 2. 安装 .NET 10 SDK

### 方法一：使用官方安装脚本（推荐）
```bash
# 下载并安装 .NET 10 SDK
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --install-dir ~/.dotnet

# 配置环境变量
export PATH="$HOME/.dotnet:$PATH"
echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> ~/.bashrc

# 验证安装
dotnet --version
# 输出: 10.0.300
```

### 方法二：使用包管理器（Ubuntu）
```bash
# 添加 Microsoft 包源
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# 更新并安装
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0
```

---

## 3. 安装 ABP CLI

```bash
# 安装 ABP CLI 工具
dotnet tool install -g Volo.Abp.Cli

# 验证安装
abp --version
# 输出: ABP CLI 10.4.0
```

---

## 4. 创建 ABP 解决方案

### 4.1 创建新项目
```bash
# 创建项目目录
mkdir -p /workspace/DddDemo
cd /workspace/DddDemo

# 使用 ABP CLI 创建解决方案
# 参数说明：
# -t app: 使用应用程序模板
# --ui none: 不使用 UI 框架（纯 API）
# --database-provider ef: 使用 Entity Framework Core
# --database-management-system PostgreSQL: 使用 PostgreSQL 数据库
# --mobile none: 不包含移动端
abp new DddDemo -t app --ui none --database-provider ef --database-management-system PostgreSQL --mobile none
```

### 4.2 项目结构说明
创建完成后，项目结构如下：
```
DddDemo/
├── aspnet-core/
│   ├── src/
│   │   ├── DddDemo.Domain.Shared/      # 领域共享层
│   │   ├── DddDemo.Domain/             # 领域层
│   │   ├── DddDemo.Application.Contracts/  # 应用契约层
│   │   ├── DddDemo.Application/        # 应用层
│   │   ├── DddDemo.EntityFrameworkCore/    # 基础设施层
│   │   ├── DddDemo.HttpApi/            # API 层
│   │   ├── DddDemo.HttpApi.Host/       # API 主机
│   │   ├── DddDemo.HttpApi.Client/     # API 客户端
│   │   └── DddDemo.DbMigrator/         # 数据库迁移工具
│   └── test/                           # 测试项目
└── README.md
```

---

## 5. 配置 PostgreSQL 数据库

### 5.1 使用 Docker 运行 PostgreSQL

创建 `docker-compose.yml` 文件：
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: ddddemo-postgres
    environment:
      POSTGRES_USER: gzzang
      POSTGRES_PASSWORD: zz123123
      POSTGRES_DB: DddDemo
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - ddddemo-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U gzzang -d DddDemo"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: ddddemo-redis
    ports:
      - "6379:6379"
    networks:
      - ddddemo-network

volumes:
  postgres_data:

networks:
  ddddemo-network:
    driver: bridge
```

### 5.2 启动 PostgreSQL
```bash
# 启动容器
docker-compose up -d

# 验证容器运行状态
docker-compose ps

# 查看日志
docker-compose logs -f postgres
```

### 5.3 配置连接字符串

编辑 `aspnet-core/src/DddDemo.HttpApi.Host/appsettings.json`：
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=DddDemo;User ID=gzzang;Password=zz123123;"
  }
}
```

编辑 `aspnet-core/src/DddDemo.DbMigrator/appsettings.json`：
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=DddDemo;User ID=gzzang;Password=zz123123;"
  }
}
```

---

## 6. 领域层设计 (Domain Layer)

### 6.1 创建订单聚合根 (Aggregate Root)

创建文件 `src/DddDemo.Domain/Orders/Order.cs`：
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;

namespace DddDemo.Orders;

/// <summary>
/// 订单聚合根 - 领域驱动设计中的聚合根实体
/// </summary>
public class Order : FullAuditedAggregateRoot<Guid>
{
    public string OrderNo { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Address ShippingAddress { get; private set; }
    public ICollection<OrderItem> Items { get; private set; }
    public string? Remark { get; private set; }

    protected Order()
    {
        Items = new List<OrderItem>();
    }

    public Order(
        Guid id,
        string orderNo,
        Guid customerId,
        Address shippingAddress,
        string? remark = null) : base(id)
    {
        OrderNo = orderNo;
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
        Remark = remark;
        Items = new List<OrderItem>();
    }

    // 领域方法
    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只能向待处理订单添加商品");

        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
            existingItem.AddQuantity(quantity);
        else
            Items.Add(new OrderItem(Guid.NewGuid(), Id, productId, productName, unitPrice, quantity));

        CalculateTotalAmount();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只能确认待处理订单");
        if (!Items.Any())
            throw new InvalidOperationException("订单必须包含至少一个商品");

        Status = OrderStatus.Confirmed;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("只能发货已确认订单");
        Status = OrderStatus.Shipped;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("只能完成已发货订单");
        Status = OrderStatus.Completed;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("已完成或已取消的订单不能再次取消");

        Status = OrderStatus.Cancelled;
        Remark = $"{Remark}\n取消原因: {reason}";
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(i => i.TotalPrice);
    }

    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Shipped = 2,
        Completed = 3,
        Cancelled = 4
    }
}
```

### 6.2 创建订单项实体

创建文件 `src/DddDemo.Domain/Orders/OrderItem.cs`：
```csharp
using System;
using Volo.Abp.Domain.Entities;

namespace DddDemo.Orders;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    protected OrderItem() { }

    public OrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("数量必须大于0", nameof(quantity));
        Quantity += quantity;
    }
}
```

### 6.3 创建地址值对象

创建文件 `src/DddDemo.Domain/Orders/Address.cs`：
```csharp
using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace DddDemo.Orders;

public class Address : ValueObject
{
    public string Province { get; private set; }
    public string City { get; private set; }
    public string District { get; private set; }
    public string Detail { get; private set; }
    public string? ZipCode { get; private set; }
    public string ContactName { get; private set; }
    public string ContactPhone { get; private set; }

    protected Address()
    {
        Province = string.Empty;
        City = string.Empty;
        District = string.Empty;
        Detail = string.Empty;
        ContactName = string.Empty;
        ContactPhone = string.Empty;
    }

    public Address(
        string province,
        string city,
        string district,
        string detail,
        string contactName,
        string contactPhone,
        string? zipCode = null)
    {
        Province = province;
        City = city;
        District = district;
        Detail = detail;
        ContactName = contactName;
        ContactPhone = contactPhone;
        ZipCode = zipCode;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Province;
        yield return City;
        yield return District;
        yield return Detail;
        yield return ContactName;
        yield return ContactPhone;
        yield return ZipCode ?? string.Empty;
    }
}
```

### 6.4 创建仓储接口

创建文件 `src/DddDemo.Domain/Orders/IOrderRepository.cs`：
```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DddDemo.Orders;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);
    Task<List<Order>> GetListByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetListByStatusAsync(Order.OrderStatus status, CancellationToken cancellationToken = default);
    Task<int> GetTodayOrderCountAsync(CancellationToken cancellationToken = default);
}
```

### 6.5 创建领域服务

创建文件 `src/DddDemo.Domain/Orders/OrderManager.cs`：
```csharp
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace DddDemo.Orders;

public class OrderManager : DomainService
{
    private readonly IOrderRepository _orderRepository;

    public OrderManager(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order> CreateAsync(
        Guid customerId,
        Address shippingAddress,
        string? remark = null)
    {
        var orderNo = await GenerateOrderNoAsync();
        var order = new Order(
            GuidGenerator.Create(),
            orderNo,
            customerId,
            shippingAddress,
            remark);

        return order;
    }

    private async Task<string> GenerateOrderNoAsync()
    {
        var today = Clock.Now.ToString("yyyyMMdd");
        var count = await _orderRepository.GetTodayOrderCountAsync();
        var sequence = (count + 1).ToString().PadLeft(6, '0');
        return $"ORD{today}{sequence}";
    }
}
```

---

## 7. 应用层设计 (Application Layer)

### 7.1 创建 DTO

创建文件 `src/DddDemo.Application.Contracts/Orders/OrderDto.cs`：
```csharp
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DddDemo.Orders;

public class OrderDto : FullAuditedEntityDto<Guid>
{
    public string OrderNo { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public int Status { get; set; }
    public decimal TotalAmount { get; set; }
    public AddressDto ShippingAddress { get; set; } = new();
    public List<OrderItemDto> Items { get; set; } = new();
    public string? Remark { get; set; }
}

public class OrderItemDto : EntityDto<Guid>
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class AddressDto
{
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}
```

创建文件 `src/DddDemo.Application.Contracts/Orders/CreateOrderDto.cs`：
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DddDemo.Orders;

public class CreateOrderDto
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public CreateAddressDto ShippingAddress { get; set; } = new();

    [Required]
    [MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = new();

    public string? Remark { get; set; }
}

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class CreateAddressDto
{
    [Required]
    [StringLength(50)]
    public string Province { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string District { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Detail { get; set; } = string.Empty;

    [StringLength(10)]
    public string? ZipCode { get; set; }

    [Required]
    [StringLength(50)]
    public string ContactName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Phone]
    public string ContactPhone { get; set; } = string.Empty;
}
```

### 7.2 创建应用服务接口

创建文件 `src/DddDemo.Application.Contracts/Orders/IOrderAppService.cs`：
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DddDemo.Orders;

public interface IOrderAppService : IApplicationService
{
    Task<OrderDto> GetAsync(Guid id);
    Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<OrderDto> CreateAsync(CreateOrderDto input);
    Task ConfirmAsync(Guid id);
    Task ShipAsync(Guid id);
    Task CompleteAsync(Guid id);
    Task CancelAsync(Guid id, string reason);
    Task<List<OrderDto>> GetListByCustomerAsync(Guid customerId);
}
```

### 7.3 实现应用服务

创建文件 `src/DddDemo.Application/Orders/OrderAppService.cs`：
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DddDemo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DddDemo.Orders;

[Authorize(DddDemoPermissions.Orders.Default)]
public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderManager _orderManager;

    public OrderAppService(
        IOrderRepository orderRepository,
        OrderManager orderManager)
    {
        _orderRepository = orderRepository;
        _orderManager = orderManager;
    }

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    public async Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _orderRepository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var orders = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(input.Sorting ?? "CreationTime DESC")
                .PageBy(input));

        return new PagedResultDto<OrderDto>(
            totalCount,
            ObjectMapper.Map<List<Order>, List<OrderDto>>(orders));
    }

    [Authorize(DddDemoPermissions.Orders.Create)]
    public async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        var address = new Address(
            input.ShippingAddress.Province,
            input.ShippingAddress.City,
            input.ShippingAddress.District,
            input.ShippingAddress.Detail,
            input.ShippingAddress.ContactName,
            input.ShippingAddress.ContactPhone,
            input.ShippingAddress.ZipCode);

        var order = await _orderManager.CreateAsync(
            input.CustomerId,
            address,
            input.Remark);

        foreach (var item in input.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
        }

        await _orderRepository.InsertAsync(order);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task ConfirmAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Confirm();
        await _orderRepository.UpdateAsync(order);
    }

    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task ShipAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Ship();
        await _orderRepository.UpdateAsync(order);
    }

    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task CompleteAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Complete();
        await _orderRepository.UpdateAsync(order);
    }

    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task CancelAsync(Guid id, string reason)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Cancel(reason);
        await _orderRepository.UpdateAsync(order);
    }

    public async Task<List<OrderDto>> GetListByCustomerAsync(Guid customerId)
    {
        var orders = await _orderRepository.GetListByCustomerIdAsync(customerId);
        return orders.ConvertAll(o => ObjectMapper.Map<Order, OrderDto>(o));
    }
}
```

---

## 8. 基础设施层设计 (Infrastructure Layer)

### 8.1 配置 DbContext

编辑 `src/DddDemo.EntityFrameworkCore/EntityFrameworkCore/DddDemoDbContext.cs`：
```csharp
using DddDemo.Orders;
using Microsoft.EntityFrameworkCore;
// ... 其他 using

public class DddDemoDbContext : AbpDbContext<DddDemoDbContext>, IIdentityDbContext, ITenantManagementDbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    // ... 其他 DbSet

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ... 模块配置

        // 订单实体配置
        builder.Entity<Order>(b =>
        {
            b.ToTable(DddDemoConsts.DbTablePrefix + "Orders", DddDemoConsts.DbSchema);
            b.Property(x => x.OrderNo).IsRequired().HasMaxLength(50);
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.Property(x => x.Remark).HasMaxLength(1000);
            b.OwnsOne(x => x.ShippingAddress, address =>
            {
                address.Property(x => x.Province).HasMaxLength(50);
                address.Property(x => x.City).HasMaxLength(50);
                address.Property(x => x.District).HasMaxLength(50);
                address.Property(x => x.Detail).HasMaxLength(500);
                address.Property(x => x.ZipCode).HasMaxLength(10);
                address.Property(x => x.ContactName).HasMaxLength(50);
                address.Property(x => x.ContactPhone).HasMaxLength(20);
            });
            b.HasIndex(x => x.OrderNo).IsUnique();
            b.HasIndex(x => x.CustomerId);
            b.HasIndex(x => x.Status);
        });

        // 订单项实体配置
        builder.Entity<OrderItem>(b =>
        {
            b.ToTable(DddDemoConsts.DbTablePrefix + "OrderItems", DddDemoConsts.DbSchema);
            b.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.HasIndex(x => x.OrderId);
        });
    }
}
```

### 8.2 实现仓储

创建文件 `src/DddDemo.EntityFrameworkCore/Orders/OrderRepository.cs`：
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DddDemo.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DddDemo.Orders;

public class OrderRepository : EfCoreRepository<DddDemoDbContext, Order, Guid>, IOrderRepository
{
    public OrderRepository(IDbContextProvider<DddDemoDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNo == orderNo, cancellationToken);
    }

    public async Task<List<Order>> GetListByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Order>> GetListByStatusAsync(Order.OrderStatus status, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTodayOrderCountAsync(CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        return await dbContext.Orders
            .CountAsync(o => o.CreationTime >= today && o.CreationTime < tomorrow, cancellationToken);
    }

    public override async Task<IQueryable<Order>> WithDetailsAsync()
    {
        var dbContext = await GetDbContextAsync();
        return dbContext.Orders.Include(o => o.Items);
    }
}
```

---

## 9. API 层设计 (HttpApi Layer)

### 9.1 创建 API 控制器

创建文件 `src/DddDemo.HttpApi/Controllers/OrdersController.cs`：
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DddDemo.Orders;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace DddDemo.Controllers;

[Route("api/orders")]
public class OrdersController : DddDemoController
{
    private readonly IOrderAppService _orderAppService;

    public OrdersController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }

    [HttpGet("{id}")]
    public Task<OrderDto> GetAsync(Guid id)
    {
        return _orderAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<OrderDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _orderAppService.GetListAsync(input);
    }

    [HttpPost]
    public Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        return _orderAppService.CreateAsync(input);
    }

    [HttpPost("{id}/confirm")]
    public Task ConfirmAsync(Guid id)
    {
        return _orderAppService.ConfirmAsync(id);
    }

    [HttpPost("{id}/ship")]
    public Task ShipAsync(Guid id)
    {
        return _orderAppService.ShipAsync(id);
    }

    [HttpPost("{id}/complete")]
    public Task CompleteAsync(Guid id)
    {
        return _orderAppService.CompleteAsync(id);
    }

    [HttpPost("{id}/cancel")]
    public Task CancelAsync(Guid id, [FromBody] CancelOrderRequest request)
    {
        return _orderAppService.CancelAsync(id, request.Reason);
    }

    [HttpGet("by-customer/{customerId}")]
    public Task<List<OrderDto>> GetListByCustomerAsync(Guid customerId)
    {
        return _orderAppService.GetListByCustomerAsync(customerId);
    }
}

public class CancelOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}
```

---

## 10. 数据库迁移

### 10.1 安装 EF Core 工具
```bash
dotnet tool install --global dotnet-ef
```

### 10.2 创建迁移
```bash
cd aspnet-core/src/DddDemo.EntityFrameworkCore

dotnet ef migrations add InitialCreate \
    --startup-project ../DddDemo.DbMigrator \
    --context DddDemoDbContext
```

### 10.3 更新数据库
```bash
# 使用 DbMigrator 项目
cd aspnet-core/src/DddDemo.DbMigrator
dotnet run

# 或使用 EF Core CLI
cd aspnet-core/src/DddDemo.EntityFrameworkCore
dotnet ef database update \
    --startup-project ../DddDemo.DbMigrator \
    --context DddDemoDbContext
```

---

## 11. 运行应用程序

### 11.1 构建项目
```bash
cd aspnet-core
dotnet build
```

### 11.2 运行 API 主机
```bash
cd src/DddDemo.HttpApi.Host
dotnet run
```

### 11.3 访问 Swagger UI
打开浏览器访问: `https://localhost:44369/swagger`

---

## 12. 项目结构

```
DddDemo/
├── aspnet-core/
│   ├── src/
│   │   ├── DddDemo.Domain.Shared/          # 领域共享层
│   │   │   └── Localization/
│   │   │       └── DddDemo/
│   │   │           ├── en.json
│   │   │           └── zh-Hans.json
│   │   │
│   │   ├── DddDemo.Domain/                 # 领域层
│   │   │   └── Orders/
│   │   │       ├── Order.cs                # 聚合根
│   │   │       ├── OrderItem.cs            # 实体
│   │   │       ├── Address.cs              # 值对象
│   │   │       ├── IOrderRepository.cs     # 仓储接口
│   │   │       └── OrderManager.cs         # 领域服务
│   │   │
│   │   ├── DddDemo.Application.Contracts/  # 应用契约层
│   │   │   ├── Orders/
│   │   │   │   ├── IOrderAppService.cs     # 应用服务接口
│   │   │   │   ├── OrderDto.cs             # DTO
│   │   │   │   └── CreateOrderDto.cs       # 创建DTO
│   │   │   └── Permissions/
│   │   │       ├── DddDemoPermissions.cs
│   │   │       └── DddDemoPermissionDefinitionProvider.cs
│   │   │
│   │   ├── DddDemo.Application/            # 应用层
│   │   │   ├── Orders/
│   │   │   │   └── OrderAppService.cs      # 应用服务实现
│   │   │   └── DddDemoApplicationMappers.cs # AutoMapper配置
│   │   │
│   │   ├── DddDemo.EntityFrameworkCore/    # 基础设施层
│   │   │   ├── EntityFrameworkCore/
│   │   │   │   └── DddDemoDbContext.cs     # DbContext
│   │   │   └── Orders/
│   │   │       └── OrderRepository.cs      # 仓储实现
│   │   │
│   │   ├── DddDemo.HttpApi/                # API 层
│   │   │   └── Controllers/
│   │   │       └── OrdersController.cs     # API 控制器
│   │   │
│   │   ├── DddDemo.HttpApi.Host/           # API 主机
│   │   │   ├── appsettings.json
│   │   │   └── Program.cs
│   │   │
│   │   └── DddDemo.DbMigrator/             # 数据库迁移工具
│   │
│   └── test/                               # 测试项目
│
├── docker-compose.yml                      # Docker Compose 配置
└── ABP-DDD-完整流程文档.md                  # 本文档
```

---

## 13. API 接口文档

### 13.1 订单管理 API

| 方法 | 路径 | 描述 | 权限 |
|------|------|------|------|
| GET | `/api/orders/{id}` | 获取订单详情 | Orders |
| GET | `/api/orders` | 获取订单列表 | Orders |
| POST | `/api/orders` | 创建订单 | Orders.Create |
| POST | `/api/orders/{id}/confirm` | 确认订单 | Orders.Update |
| POST | `/api/orders/{id}/ship` | 发货 | Orders.Update |
| POST | `/api/orders/{id}/complete` | 完成订单 | Orders.Update |
| POST | `/api/orders/{id}/cancel` | 取消订单 | Orders.Update |
| GET | `/api/orders/by-customer/{customerId}` | 获取客户订单 | Orders |

### 13.2 创建订单示例请求
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "shippingAddress": {
    "province": "广东省",
    "city": "深圳市",
    "district": "南山区",
    "detail": "科技园南区",
    "contactName": "张三",
    "contactPhone": "13800138000",
    "zipCode": "518000"
  },
  "items": [
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "productName": "iPhone 15 Pro",
      "unitPrice": 8999.00,
      "quantity": 1
    }
  ],
  "remark": "请尽快发货"
}
```

### 13.3 订单响应示例
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
  "orderNo": "ORD20250520000001",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": 0,
  "totalAmount": 8999.00,
  "shippingAddress": {
    "province": "广东省",
    "city": "深圳市",
    "district": "南山区",
    "detail": "科技园南区",
    "contactName": "张三",
    "contactPhone": "13800138000",
    "zipCode": "518000"
  },
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "productName": "iPhone 15 Pro",
      "unitPrice": 8999.00,
      "quantity": 1,
      "totalPrice": 8999.00
    }
  ],
  "remark": "请尽快发货",
  "creationTime": "2025-05-20T10:00:00Z"
}
```

---

## 14. DDD 核心概念实践

### 14.1 分层架构
- **领域层 (Domain)**: 包含核心业务逻辑、实体、值对象、领域服务
- **应用层 (Application)**: 协调领域对象完成用例，定义 DTO 和应用服务
- **基础设施层 (Infrastructure)**: 实现仓储、数据库访问
- **API 层 (HttpApi)**: 提供 REST API 接口

### 14.2 领域驱动设计模式
- **聚合根 (Aggregate Root)**: `Order` 是聚合根，管理 `OrderItem` 集合
- **实体 (Entity)**: `OrderItem` 是聚合内的实体
- **值对象 (Value Object)**: `Address` 是值对象，没有唯一标识
- **仓储 (Repository)**: `IOrderRepository` 和 `OrderRepository`
- **领域服务 (Domain Service)**: `OrderManager` 处理跨实体的业务逻辑

### 14.3 状态机模式
订单状态流转：
```
Pending -> Confirmed -> Shipped -> Completed
   |
   v
Cancelled
```

---

## 15. 总结

本项目展示了如何使用 ABP 框架和 .NET 10 构建一个完整的领域驱动设计应用程序：

1. ✅ 使用 ABP CLI 创建项目模板
2. ✅ 配置 PostgreSQL 数据库连接
3. ✅ 实现 DDD 分层架构
4. ✅ 创建聚合根、实体、值对象
5. ✅ 实现仓储模式
6. ✅ 创建应用服务和 API 控制器
7. ✅ 配置权限和本地化
8. ✅ 数据库迁移

项目代码位于: `/workspace/DddDemo/`
