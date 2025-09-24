using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs.Convertions;

public static class OrderConversion
{
    public static Order ToEntity(OrderDTO order) => new()
    {
        Id = order.Id,
        ClientId = order.ClientId,
        ProductId = order.ProductId,
        OrderDate = order.OrderDate,
        PurchaseQuantity = order.PurchaseQuantity
    };

    public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntinty(Order? order, IEnumerable<Order>? orders)
    {
        // Single order conversion
        if(order is not null || orders is null)
        {
            var singleOrder = new OrderDTO(
                order!.Id,
                order.ClientId,
                order.ProductId,
                order.PurchaseQuantity,
                order.OrderDate);

            return (singleOrder, null);
        }

        // Multiple orders conversion
        if (orders is not null || order is null)
        {
            var _orders =  orders!.Select(o => 
            new OrderDTO(
                o.Id,
                o.ClientId,
                o.ProductId,
                o.PurchaseQuantity,
                o.OrderDate));

            return (null, _orders);
        }
        return (null, null);
    }
}
