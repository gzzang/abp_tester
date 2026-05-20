using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Values;

namespace DddDemo.Orders;

/// <summary>
/// 订单聚合根 - 领域驱动设计中的聚合根实体
/// </summary>
public class Order : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; private set; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// 收货地址
    /// </summary>
    public Address ShippingAddress { get; private set; }

    /// <summary>
    /// 订单项集合
    /// </summary>
    public ICollection<OrderItem> Items { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
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

    /// <summary>
    /// 添加订单项
    /// </summary>
    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("只能向待处理订单添加商品");
        }

        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            var item = new OrderItem(Guid.NewGuid(), Id, productId, productName, unitPrice, quantity);
            Items.Add(item);
        }

        CalculateTotalAmount();
    }

    /// <summary>
    /// 移除订单项
    /// </summary>
    public void RemoveItem(Guid itemId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("只能从待处理订单移除商品");
        }

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            CalculateTotalAmount();
        }
    }

    /// <summary>
    /// 确认订单
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("只能确认待处理订单");
        }

        if (!Items.Any())
        {
            throw new InvalidOperationException("订单必须包含至少一个商品");
        }

        Status = OrderStatus.Confirmed;
    }

    /// <summary>
    /// 发货
    /// </summary>
    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("只能发货已确认订单");
        }

        Status = OrderStatus.Shipped;
    }

    /// <summary>
    /// 完成订单
    /// </summary>
    public void Complete()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("只能完成已发货订单");
        }

        Status = OrderStatus.Completed;
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("已完成或已取消的订单不能再次取消");
        }

        Status = OrderStatus.Cancelled;
        Remark = $"{Remark}\n取消原因: {reason}";
    }

    /// <summary>
    /// 计算订单总金额
    /// </summary>
    private void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(i => i.TotalPrice);
    }
    /// <summary>
    /// 订单状态枚举
    /// </summary>
    public enum OrderStatus
    {
        Pending = 0,      // 待处理
        Confirmed = 1,    // 已确认
        Shipped = 2,      // 已发货
        Completed = 3,    // 已完成
        Cancelled = 4     // 已取消
    }
}
