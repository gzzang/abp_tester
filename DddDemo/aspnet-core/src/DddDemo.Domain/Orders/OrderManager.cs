using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace DddDemo.Orders;

/// <summary>
/// 订单领域服务 - 领域驱动设计中的领域服务
/// </summary>
public class OrderManager : DomainService
{
    private readonly IOrderRepository _orderRepository;

    public OrderManager(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// 创建新订单
    /// </summary>
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

    /// <summary>
    /// 生成订单号
    /// </summary>
    private async Task<string> GenerateOrderNoAsync()
    {
        var today = Clock.Now.ToString("yyyyMMdd");
        var count = await _orderRepository.GetTodayOrderCountAsync();
        var sequence = (count + 1).ToString().PadLeft(6, '0');
        return $"ORD{today}{sequence}";
    }

    /// <summary>
    /// 批量确认订单
    /// </summary>
    public async Task BatchConfirmAsync(Guid[] orderIds)
    {
        foreach (var orderId in orderIds)
        {
            var order = await _orderRepository.GetAsync(orderId);
            order.Confirm();
            await _orderRepository.UpdateAsync(order);
        }
    }
}
