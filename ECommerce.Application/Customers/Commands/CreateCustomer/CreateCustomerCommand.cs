using ECommerce.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(
        string FullName,
        string Email,
        bool IsVip
    ) : IRequest<CustomerDto>;
}
