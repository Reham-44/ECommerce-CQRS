//using ECommerce.Application.DTOs;
//using ECommerce.Application.Interfaces.Repositories;
//using ECommerce.Application.Interfaces.Services;
//using ECommerce.Domain.Entities;
//using ECommerce.Domain.Enums;

//namespace ECommerce.Application.Services;

//public class CustomerService : ICustomerService
//{
//    private readonly ICustomerRepo _customerRepo;

//    public CustomerService(ICustomerRepo customerRepo)
//    {
//        _customerRepo = customerRepo;
//    }

//    public async Task<CustomerDto?> GetById(int id)
//    {
//        var customer = await _customerRepo.GetById(id);

//        if (customer == null)
//            return null;

//        return new CustomerDto
//        {
//            Id = customer.Id,
//            FullName = customer.FullName,
//            Email = customer.Email,
//            IsVip = customer.IsVip,

//            Orders = customer.Orders.Select(o => new CustomerOrderDto
//            {
//                Id = o.Id,
//                CreatedAt = o.CreatedAt,
//                Status = o.Status,
//                TotalAmount = o.TotalAmount
//            }).ToList()
//        };
//    }

//    public async Task<(bool Success, string? Error, CustomerDto? Customer)> Create(
//        CreateCustomerDto dto)
//    {
//        if (string.IsNullOrWhiteSpace(dto.FullName))
//            return (false, "Full name is required.", null);

//        if (string.IsNullOrWhiteSpace(dto.Email) ||
//            !dto.Email.Contains("@"))
//        {
//            return (false, "A valid email address is required.", null);
//        }

//        if (await _customerRepo.EmailExists(dto.Email))
//            return (false, "Email is already registered.", null);

//        var customer = new Customer
//        {
//            FullName = dto.FullName,
//            Email = dto.Email,
//            IsVip = dto.IsVip
//        };

//        await _customerRepo.Add(customer);
//        await _customerRepo.SaveChanges();

//        var result = new CustomerDto
//        {
//            Id = customer.Id,
//            FullName = customer.FullName,
//            Email = customer.Email,
//            IsVip = customer.IsVip
//        };

//        return (true, null, result);
//    }

//    public async Task<(bool Success, string? Error)> UpgradeToVip(int id)
//    {
//        var customer = await _customerRepo.GetById(id);

//        if (customer == null)
//            return (false, "Customer not found.");

//        var totalSpent = customer.Orders
//            .Where(o => o.Status == OrderStatus.Paid)
//            .Sum(o => o.TotalAmount);

//        if (totalSpent < 500m)
//        {
//            return (
//                false,
//                $"Customer does not qualify for VIP. Total spend {totalSpent:C} is less than required $500.00"
//            );
//        }

//        customer.IsVip = true;

//        await _customerRepo.SaveChanges();

//        return (true, null);
//    }
//}