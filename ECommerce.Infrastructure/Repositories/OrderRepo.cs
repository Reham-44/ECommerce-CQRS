using Azure.Core;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class OrderRepo : IOrderRepo
    {
    private readonly AppDbContext _context;

    public OrderRepo(AppDbContext context)
    {
        _context = context;
    }

 
    public async Task Add(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> GetById(int id)
    {
        var order = await _context.Orders
                   .Include(o => o.Items)
                   .ThenInclude(i => i.Product)
                   .Include(o => o.Payment)
                   .FirstOrDefaultAsync(o => o.Id == id);
        return order;
    }

    public async Task<Product?> GetProductById(int productId)
    {
        var product = await _context.Products.FindAsync(productId);  
        return product;
    }

    public async Task<Coupon?> GetCouponByCode(string code)
    {
        var coupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code.ToUpper() == code.ToUpper() && c.IsActive);
        return coupon;
    }

    public async Task AddPayment(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Customer?> GetCustomerById(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);

        return customer;
    }

    public async Task<List<Order>> GetOrdersByCustomerId(int customerId)
    {
        var orders = await _context.Orders
              .Include(o => o.Items)
              .Where(o => o.CustomerId == customerId)
              .ToListAsync();
        return orders;
    }
}