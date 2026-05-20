using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using DddDemo.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DddDemo.Orders;

/// <summary>
/// 订单仓储实现
/// </summary>
public class OrderRepository : EfCoreRepository<DddDemoDbContext, Order, Guid>, IOrderRepository
{
    public OrderRepository(IDbContextProvider<DddDemoDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// 根据订单号获取订单
    /// </summary>
    public async Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNo == orderNo, cancellationToken);
    }

    /// <summary>
    /// 根据客户ID获取订单列表
    /// </summary>
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

    /// <summary>
    /// 根据状态获取订单列表
    /// </summary>
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

    /// <summary>
    /// 获取今日订单数量
    /// </summary>
    public async Task<int> GetTodayOrderCountAsync(CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        return await dbContext.Orders
            .CountAsync(o => o.CreationTime >= today && o.CreationTime < tomorrow, cancellationToken);
    }

    /// <summary>
    /// 重写获取Queryable，包含订单项
    /// </summary>
    public override async Task<IQueryable<Order>> WithDetailsAsync()
    {
        var dbContext = await GetDbContextAsync();
        return dbContext.Orders.Include(o => o.Items);
    }
}
