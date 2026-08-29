using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IOrderRepo
{
    Task<Order?> GetById(int id);

    Task<List<Order>> GetOrdersByCustomerId(int customerId);

    Task<Customer?> GetCustomerById(int customerId);

    Task<Product?> GetProductById(int productId);

    Task<Coupon?> GetCouponByCode(string code);

    Task Add(Order order);

    Task AddPayment(Payment payment);

    Task SaveChanges();
}