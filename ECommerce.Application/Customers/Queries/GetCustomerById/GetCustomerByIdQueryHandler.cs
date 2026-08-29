using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Customers.Queries.GetCustomerById
{
       public class GetCustomerByIdQueryHandler
        : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
    {
        private readonly IReadCustomerRepo _customerRepo;

        public GetCustomerByIdQueryHandler(
            IReadCustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<CustomerDto?> Handle(
            GetCustomerByIdQuery request,
            CancellationToken cancellationToken)
        {
            var customer = await _customerRepo.GetById(request.Id);

            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                IsVip = customer.IsVip,
                Orders = customer.Orders.Select(o => new CustomerOrderDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount
                }).ToList()
            };
        }
    }
}
