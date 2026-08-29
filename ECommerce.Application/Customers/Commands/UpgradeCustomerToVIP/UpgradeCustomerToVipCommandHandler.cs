using ECommerce.Application.Customers.Commands.UpgradeCustomerToVIP;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Enums;
using MediatR;

public class UpgradeCustomerToVipCommandHandler
    : IRequestHandler<
        UpgradeCustomerToVipCommand,
        (bool Success, string? Error)>
{
    private readonly IReadCustomerRepo _readCustomerRepo;
    private readonly IWriteCustomerRepo _writeCustomerRepo;

    public UpgradeCustomerToVipCommandHandler(
        IReadCustomerRepo readCustomerRepo,
        IWriteCustomerRepo writeCustomerRepo)
    {
        _readCustomerRepo = readCustomerRepo;
        _writeCustomerRepo = writeCustomerRepo;
    }

    public async Task<(bool Success, string? Error)> Handle(
        UpgradeCustomerToVipCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _readCustomerRepo.GetById(request.Id);

        if (customer == null)
            return (false, "Customer not found.");

        var totalSpent = customer.Orders
            .Where(o => o.Status == OrderStatus.Paid)
            .Sum(o => o.TotalAmount);

        if (totalSpent < 500m)
        {
            return (
                false,
                $"Customer does not qualify for VIP. " +
                $"Total spend {totalSpent:C} is less than required $500.00"
            );
        }

        customer.IsVip = true;

        await _writeCustomerRepo.SaveChanges();

        return (true, null);
    }
}