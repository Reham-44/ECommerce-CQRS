using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepo _productRepo;

    public ProductService(IProductRepo productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<List<ProductDto>> GetAll()
    {
        var products = await _productRepo.GetAll();

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetById(int id)
    {
        var product = await _productRepo.GetById(id);

        if (product == null)
            return null;

        return MapToDto(product);
    }

    public async Task<(bool Success, string? Error, ProductDto? Product)> Create(
        CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, "Product name is required.", null);

        if (string.IsNullOrWhiteSpace(dto.SKU))
            return (false, "Product SKU is required.", null);

        if (dto.Price <= 0)
            return (false, "Product price must be greater than zero.", null);

        if (dto.StockQuantity < 0)
            return (false, "Stock quantity cannot be negative.", null);

        var sku = dto.SKU.Trim().ToUpper();

        if (await _productRepo.SkuExists(sku))
        {
            return (
                false,
                $"Product with SKU '{dto.SKU}' already exists.",
                null
            );
        }

        var product = new Product
        {
            Name = dto.Name.Trim(),
            SKU = sku,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity
        };

        await _productRepo.Add(product);
        await _productRepo.SaveChanges();

        return (true, null, MapToDto(product));
    }

    public async Task<(bool Success, string? Error)> Update(
        int id,
        CreateProductDto dto)
    {
        var existing = await _productRepo.GetById(id);

        if (existing == null)
            return (false, $"Product with ID {id} not found.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, "Product name is required.");

        if (string.IsNullOrWhiteSpace(dto.SKU))
            return (false, "Product SKU is required.");

        if (dto.Price <= 0)
            return (false, "Price must be positive.");

        if (dto.StockQuantity < 0)
            return (false, "Stock quantity cannot be negative.");

        var sku = dto.SKU.Trim().ToUpper();

        // Don't consider the current product when checking SKU
        if (!existing.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase)
            && await _productRepo.SkuExists(sku))
        {
            return (
                false,
                $"Product with SKU '{dto.SKU}' already exists."
            );
        }

        existing.Name = dto.Name.Trim();
        existing.SKU = sku;
        existing.Price = dto.Price;
        existing.StockQuantity = dto.StockQuantity;

        _productRepo.Update(existing);
        await _productRepo.SaveChanges();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> Delete(int id)
    {
        var product = await _productRepo.GetById(id);

        if (product == null)
            return (false, $"Product with ID {id} not found.");

        _productRepo.Delete(product);
        await _productRepo.SaveChanges();

        return (true, null);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            StockQuantity = product.StockQuantity
        };
    }
}