using ECommerce.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Customers.Queries.GetCustomerById
{
    public record GetCustomerByIdQuery(int Id) : IRequest<CustomerDto?>;
}
