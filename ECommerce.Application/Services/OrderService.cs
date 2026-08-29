using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepo _orderRepo;

    public OrderService(IOrderRepo orderRepo)
    {
        _orderRepo = orderRepo;
    }

    public async Task<OrderDto?> GetById(int id)
    {
        var order = await _orderRepo.GetById(id);

        if (order == null)
            return null;

        return MapToDto(order);
    }

    public async Task<List<OrderDto>> GetCustomerOrders(int customerId)
    {
        var orders = await _orderRepo.GetOrdersByCustomerId(customerId);

        return orders.Select(MapToDto).ToList();
    }

    public async Task<(bool Success, string? Error)> CancelOrder(int id)
    {
        var order = await _orderRepo.GetById(id);

        if (order == null)
            return (false, "Order not found");

        if (order.Status == OrderStatus.Cancelled)
            return (false, "Order is already cancelled");

        if (order.Status == OrderStatus.Paid)
        {
            foreach (var item in order.Items)
            {
                var product = await _orderRepo.GetProductById(item.ProductId);

                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }
        }

        order.Status = OrderStatus.Cancelled;

        await _orderRepo.SaveChanges();

        return (true, null);
    }

    public async Task<(bool Success, string? Error, OrderDto? Result)>
        Checkout(CreateOrderDto request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            return (false, "Cannot checkout an empty order.", null);
        }

        var customer = await _orderRepo.GetCustomerById(request.CustomerId);

        if (customer == null)
        {
            return (
                false,
                $"Customer with ID {request.CustomerId} not found.",
                null
            );
        }

        decimal subtotal = 0m;

        var orderItems = new List<OrderItem>();

        foreach (var itemDto in request.Items)
        {
            if (itemDto.Quantity <= 0)
            {
                return (
                    false,
                    "Product quantity must be at least 1.",
                    null
                );
            }

            var product = await _orderRepo.GetProductById(itemDto.ProductId);

            if (product == null)
            {
                return (
                    false,
                    $"Product with ID {itemDto.ProductId} not found.",
                    null
                );
            }

            if (product.StockQuantity < itemDto.Quantity)
            {
                return (
                    false,
                    $"Insufficient stock for product '{product.Name}'. " +
                    $"Available: {product.StockQuantity}, " +
                    $"Requested: {itemDto.Quantity}",
                    null
                );
            }

            subtotal += product.Price * itemDto.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                Product = product
            });

            product.StockQuantity -= itemDto.Quantity;
        }

        decimal discount = 0m;

        if (customer.IsVip)
        {
            discount += Math.Round(subtotal * 0.15m, 2);
        }

        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var coupon = await _orderRepo.GetCouponByCode(request.CouponCode);

            if (coupon == null)
            {
                return (
                    false,
                    $"Invalid or inactive coupon code '{request.CouponCode}'.",
                    null
                );
            }

            discount += Math.Round(
                subtotal * (coupon.DiscountPercentage / 100m),
                2
            );
        }

        if (discount > subtotal)
        {
            discount = subtotal;
        }

        var netAmount = subtotal - discount;

        var tax = Math.Round(netAmount * 0.14m, 2);

        var shipping = netAmount >= 1000m
            ? 0m
            : 75m;

        var finalTotal = netAmount + tax + shipping;

        if (finalTotal > 50000m)
        {
            return (
                false,
                "Payment processing failed. Amount exceeds limit.",
                null
            );
        }

        var txRef =
            $"TX-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var order = new Order
        {
            CustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Paid,
            Subtotal = subtotal,
            DiscountAmount = discount,
            TaxAmount = tax,
            ShippingFee = shipping,
            TotalAmount = finalTotal,
            Items = orderItems
        };

        var payment = new Payment
        {
            Order = order,
            Amount = finalTotal,
            PaymentDate = DateTime.UtcNow,
            TransactionReference = txRef,
            IsSuccess = true
        };

        await _orderRepo.Add(order);
        await _orderRepo.AddPayment(payment);
        await _orderRepo.SaveChanges();

        return (true, null, MapToDto(order));
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            ShippingFee = order.ShippingFee,
            TotalAmount = order.TotalAmount,

            Items = order.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList(),

            Payment = order.Payment == null
                ? null
                : new PaymentDto
                {
                    Id = order.Payment.Id,
                    Amount = order.Payment.Amount,
                    PaymentDate = order.Payment.PaymentDate,
                    TransactionReference =
                        order.Payment.TransactionReference,
                    IsSuccess = order.Payment.IsSuccess
                }
        };
    }
}