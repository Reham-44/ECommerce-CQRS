using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderDto?> GetById(int id);

    Task<List<OrderDto>> GetCustomerOrders(int customerId);

    Task<(bool Success, string? Error)> CancelOrder(int id);

    Task<(bool Success, string? Error, OrderDto? Result)> Checkout(
        CreateOrderDto request);
}