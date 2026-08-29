using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAll();

    Task<ProductDto?> GetById(int id);

    Task<(bool Success, string? Error, ProductDto? Product)> Create(
        CreateProductDto dto);

    Task<(bool Success, string? Error)> Update(
        int id,
        CreateProductDto dto);

    Task<(bool Success, string? Error)> Delete(int id);
}