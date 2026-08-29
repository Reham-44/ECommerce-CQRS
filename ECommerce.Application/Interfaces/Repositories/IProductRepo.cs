using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IProductRepo
{
    Task<List<Product>> GetAll();

    Task<Product?> GetById(int id);

    Task<bool> SkuExists(string sku);

    Task Add(Product product);

    void Update(Product product);

    void Delete(Product product);

    Task SaveChanges();
}