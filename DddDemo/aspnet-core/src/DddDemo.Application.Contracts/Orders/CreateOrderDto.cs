using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DddDemo.Orders;

/// <summary>
/// 创建订单DTO
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// 客户ID
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 收货地址
    /// </summary>
    [Required]
    public CreateAddressDto ShippingAddress { get; set; } = new();

    /// <summary>
    /// 订单项
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 创建订单项DTO
/// </summary>
public class CreateOrderItemDto
{
    /// <summary>
    /// 产品ID
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 单价
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

/// <summary>
/// 创建地址DTO
/// </summary>
public class CreateAddressDto
{
    /// <summary>
    /// 省份
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    [Required]
    [StringLength(50)]
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// 详细地址
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// 邮编
    /// </summary>
    [StringLength(10)]
    public string? ZipCode { get; set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人电话
    /// </summary>
    [Required]
    [StringLength(20)]
    [Phone]
    public string ContactPhone { get; set; } = string.Empty;
}
