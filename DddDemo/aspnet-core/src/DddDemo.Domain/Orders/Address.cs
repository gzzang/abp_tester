using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace DddDemo.Orders;

/// <summary>
/// 地址值对象 - 领域驱动设计中的值对象
/// </summary>
public class Address : ValueObject
{
    /// <summary>
    /// 省份
    /// </summary>
    public string Province { get; private set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; private set; }

    /// <summary>
    /// 区县
    /// </summary>
    public string District { get; private set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    public string Detail { get; private set; }

    /// <summary>
    /// 邮编
    /// </summary>
    public string? ZipCode { get; private set; }

    /// <summary>
    /// 联系人姓名
    /// </summary>
    public string ContactName { get; private set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    public string ContactPhone { get; private set; }

    protected Address()
    {
        Province = string.Empty;
        City = string.Empty;
        District = string.Empty;
        Detail = string.Empty;
        ContactName = string.Empty;
        ContactPhone = string.Empty;
    }

    public Address(
        string province,
        string city,
        string district,
        string detail,
        string contactName,
        string contactPhone,
        string? zipCode = null)
    {
        Province = province;
        City = city;
        District = district;
        Detail = detail;
        ContactName = contactName;
        ContactPhone = contactPhone;
        ZipCode = zipCode;
    }

    /// <summary>
    /// 获取完整地址字符串
    /// </summary>
    public string GetFullAddress()
    {
        return $"{Province}{City}{District}{Detail}";
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Province;
        yield return City;
        yield return District;
        yield return Detail;
        yield return ContactName;
        yield return ContactPhone;
        yield return ZipCode ?? string.Empty;
    }
}
