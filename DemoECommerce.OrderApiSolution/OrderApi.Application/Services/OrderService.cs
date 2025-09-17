using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Convertions;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Services;

public class OrderService(IOrder orderInterface, HttpClient httpClient,
    ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
{
    // Get product
    public async Task<ProductDTO> GetProduct(int productId)
    {
        // Call Product Api using HttpClient
        // Redirect this call to the API Gateway since product Api does not respond to outsiders
        HttpResponseMessage getProduct = await httpClient.GetAsync($"/api/products/{productId}");
        if (!getProduct.IsSuccessStatusCode)
            return null!;

        ProductDTO? product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
        return product!;
    }

    // Get user 
    public async Task<AppUserDTO> GetUser(int userId)
    {
        // Call Product Api using HttpClient
        // Redirect this call to the API Gateway since product Api does not respond to outsiders
        HttpResponseMessage getUser = await httpClient.GetAsync($"/api/products/{userId}");
        if (!getUser.IsSuccessStatusCode)
            return null!;

        AppUserDTO? user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return user!;
    }

    // get order details by id
    public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
    {
        // Prepare Order 
        Order? order = await orderInterface.FindByIdAsync(orderId);
        if (order is null || order!.Id <= 0)
            return null!;

        // Get retry pipeline
        ResiliencePipeline retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

        // Prepare Product
        var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

        // Prepare Client
        var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

        // Populate order Details
        return new OrderDetailsDTO(
            order.Id,
            productDTO.Id,
            appUserDTO.Id, 
            appUserDTO.Name,
            appUserDTO.Email,
            appUserDTO.Address,
            appUserDTO.TelephoneNumber,
            productDTO.Name,
            order.PurchaseQuantity,
            productDTO.Price,
            productDTO.Quantity * order.PurchaseQuantity,
            order.OrderDate
            );
    }

    // get orders by client id
    public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
    {
        // Get all cClient's orders
        var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
        if(!orders.Any()) return null!;

        // Convert from entity to DTO
        var (_, _orders) = OrderConversion.FromEntinty(null, orders);
        return _orders!;

    }
}
