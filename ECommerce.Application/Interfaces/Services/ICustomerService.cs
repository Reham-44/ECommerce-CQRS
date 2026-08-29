using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerDto?> GetById(int id);

    Task<(bool Success, string? Error, CustomerDto? Customer)> Create(
        CreateCustomerDto dto);

    Task<(bool Success, string? Error)> UpgradeToVip(int id);
}