using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DddDemo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DddDemo.Orders;

/// <summary>
/// 订单应用服务实现
/// </summary>
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

    /// <summary>
    /// 获取订单详情
    /// </summary>
    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    /// <summary>
    /// 获取订单列表
    /// </summary>
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

    /// <summary>
    /// 创建订单
    /// </summary>
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

    /// <summary>
    /// 确认订单
    /// </summary>
    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task ConfirmAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Confirm();
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// 发货
    /// </summary>
    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task ShipAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Ship();
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// 完成订单
    /// </summary>
    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task CompleteAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Complete();
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    [Authorize(DddDemoPermissions.Orders.Update)]
    public async Task CancelAsync(Guid id, string reason)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Cancel(reason);
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// 根据客户ID获取订单
    /// </summary>
    public async Task<List<OrderDto>> GetListByCustomerAsync(Guid customerId)
    {
        var orders = await _orderRepository.GetListByCustomerIdAsync(customerId);
        return orders.ConvertAll(o => ObjectMapper.Map<Order, OrderDto>(o));
    }
}
