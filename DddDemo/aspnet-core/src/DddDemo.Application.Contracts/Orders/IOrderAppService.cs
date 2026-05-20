using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DddDemo.Orders;

/// <summary>
/// 订单应用服务接口
/// </summary>
public interface IOrderAppService : IApplicationService
{
    /// <summary>
    /// 获取订单详情
    /// </summary>
    Task<OrderDto> GetAsync(Guid id);

    /// <summary>
    /// 获取订单列表
    /// </summary>
    Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input);

    /// <summary>
    /// 创建订单
    /// </summary>
    Task<OrderDto> CreateAsync(CreateOrderDto input);

    /// <summary>
    /// 确认订单
    /// </summary>
    Task ConfirmAsync(Guid id);

    /// <summary>
    /// 发货
    /// </summary>
    Task ShipAsync(Guid id);

    /// <summary>
    /// 完成订单
    /// </summary>
    Task CompleteAsync(Guid id);

    /// <summary>
    /// 取消订单
    /// </summary>
    Task CancelAsync(Guid id, string reason);

    /// <summary>
    /// 根据客户ID获取订单
    /// </summary>
    Task<List<OrderDto>> GetListByCustomerAsync(Guid customerId);
}
