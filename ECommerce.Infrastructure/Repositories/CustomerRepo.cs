using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ReadCustomerRepo : IReadCustomerRepo
    {
    private readonly AppReadDbContext _Readcontext;


    public ReadCustomerRepo(AppReadDbContext ReadContext)
    {
        _Readcontext = ReadContext;
    }

    public async Task<Customer?> GetById(int id)
    {
        return await _Readcontext.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> EmailExists(string email)
    {
        return await _Readcontext.Customers
            .AnyAsync(c => c.Email.ToLower() == email.ToLower());
    }
}


public class WriteCustomerRepo : IWriteCustomerRepo
{
   private readonly AppWriteDbContext _Writecontext;


    public WriteCustomerRepo(AppWriteDbContext WriteContext)
    {
        _Writecontext = WriteContext;
    }
    public async Task Add(Customer customer)
    {
        await _Writecontext.Customers.AddAsync(customer);
    }

    public async Task SaveChanges()
    {
        await _Writecontext.SaveChangesAsync();
    }
}