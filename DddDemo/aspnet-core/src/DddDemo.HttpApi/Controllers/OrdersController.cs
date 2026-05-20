using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DddDemo.Orders;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace DddDemo.Controllers;

/// <summary>
/// 订单 API 控制器
/// </summary>
[Route("api/orders")]
public class OrdersController : DddDemoController
{
    private readonly IOrderAppService _orderAppService;

    public OrdersController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    [HttpGet("{id}")]
    public Task<OrderDto> GetAsync(Guid id)
    {
        return _orderAppService.GetAsync(id);
    }

    /// <summary>
    /// 获取订单列表
    /// </summary>
    [HttpGet]
    public Task<PagedResultDto<OrderDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _orderAppService.GetListAsync(input);
    }

    /// <summary>
    /// 创建订单
    /// </summary>
    [HttpPost]
    public Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        return _orderAppService.CreateAsync(input);
    }

    /// <summary>
    /// 确认订单
    /// </summary>
    [HttpPost("{id}/confirm")]
    public Task ConfirmAsync(Guid id)
    {
        return _orderAppService.ConfirmAsync(id);
    }

    /// <summary>
    /// 发货
    /// </summary>
    [HttpPost("{id}/ship")]
    public Task ShipAsync(Guid id)
    {
        return _orderAppService.ShipAsync(id);
    }

    /// <summary>
    /// 完成订单
    /// </summary>
    [HttpPost("{id}/complete")]
    public Task CompleteAsync(Guid id)
    {
        return _orderAppService.CompleteAsync(id);
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    [HttpPost("{id}/cancel")]
    public Task CancelAsync(Guid id, [FromBody] CancelOrderRequest request)
    {
        return _orderAppService.CancelAsync(id, request.Reason);
    }

    /// <summary>
    /// 根据客户ID获取订单
    /// </summary>
    [HttpGet("by-customer/{customerId}")]
    public Task<List<OrderDto>> GetListByCustomerAsync(Guid customerId)
    {
        return _orderAppService.GetListByCustomerAsync(customerId);
    }
}

/// <summary>
/// 取消订单请求
/// </summary>
public class CancelOrderRequest
{
    /// <summary>
    /// 取消原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
