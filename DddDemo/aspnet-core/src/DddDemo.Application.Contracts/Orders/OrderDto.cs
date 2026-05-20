using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DddDemo.Orders;

/// <summary>
/// 订单DTO
/// </summary>
public class OrderDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 收货地址
    /// </summary>
    public AddressDto ShippingAddress { get; set; } = new();

    /// <summary>
    /// 订单项
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 订单项DTO
/// </summary>
public class OrderItemDto : EntityDto<Guid>
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 总价
    /// </summary>
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// 地址DTO
/// </summary>
public class AddressDto
{
    /// <summary>
    /// 省份
    /// </summary>
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// 详细地址
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// 邮编
    /// </summary>
    public string? ZipCode { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人电话
    /// </summary>
    public string ContactPhone { get; set; } = string.Empty;
}
