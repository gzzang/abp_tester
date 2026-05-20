using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DddDemo.Orders;

/// <summary>
/// 订单仓储接口 - 领域驱动设计中的仓储模式
/// </summary>
public interface IOrderRepository : IRepository<Order, Guid>
{
    /// <summary>
    /// 根据订单号获取订单
    /// </summary>
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据客户ID获取订单列表
    /// </summary>
    Task<List<Order>> GetListByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据状态获取订单列表
    /// </summary>
    Task<List<Order>> GetListByStatusAsync(Order.OrderStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取今日订单数量
    /// </summary>
    Task<int> GetTodayOrderCountAsync(CancellationToken cancellationToken = default);
}
