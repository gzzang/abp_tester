using System.Collections.Generic;
using DddDemo.Orders;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace DddDemo;

[Mapper]
public partial class DddDemoApplicationMappers
{
    /* You can configure your Mapperly mapping configuration here.
     * Alternatively, you can split your mapping configurations
     * into multiple mapper classes for a better organization. */

    public partial OrderDto MapOrderToDto(Order order);
    public partial IList<OrderDto> MapOrdersToDtoList(IList<Order> orders);
    public partial OrderItemDto MapOrderItemToDto(OrderItem orderItem);
}
