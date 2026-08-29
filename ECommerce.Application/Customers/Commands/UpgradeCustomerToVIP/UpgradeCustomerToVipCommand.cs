using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Customers.Commands.UpgradeCustomerToVIP
{
    public record UpgradeCustomerToVipCommand(int Id)
      : IRequest<(bool Success, string? Error)>;
}
