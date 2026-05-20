using System;
using Volo.Abp.Domain.Entities;

namespace DddDemo.Orders;

/// <summary>
/// 订单项实体 - 聚合内的实体
/// </summary>
public class OrderItem : Entity<Guid>
{
    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// 产品ID
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// 总价
    /// </summary>
    public decimal TotalPrice => UnitPrice * Quantity;

    protected OrderItem()
    {
    }

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

    /// <summary>
    /// 增加数量
    /// </summary>
    public void AddQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("数量必须大于0", nameof(quantity));
        }

        Quantity += quantity;
    }

    /// <summary>
    /// 设置数量
    /// </summary>
    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("数量必须大于0", nameof(quantity));
        }

        Quantity = quantity;
    }
}
