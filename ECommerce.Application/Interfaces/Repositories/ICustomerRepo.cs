using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IReadCustomerRepo
{
    Task<Customer?> GetById(int id);
    Task<bool> EmailExists(string email);

}

public interface IWriteCustomerRepo
{

    Task Add(Customer customer);
    Task SaveChanges();
}