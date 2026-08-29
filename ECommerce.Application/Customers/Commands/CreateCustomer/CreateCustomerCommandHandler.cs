using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler
        : IRequestHandler<CreateCustomerCommand, CustomerDto>
    {
        private readonly IWriteCustomerRepo _customerRepo;
        private readonly IReadCustomerRepo _ReadcustomerRepo;


        public CreateCustomerCommandHandler(IWriteCustomerRepo customerRepo,IReadCustomerRepo readCustomerRepo)
        {
            _customerRepo = customerRepo;
            _ReadcustomerRepo = readCustomerRepo;
        }

        public async Task<CustomerDto> Handle(
            CreateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.");

            if (string.IsNullOrWhiteSpace(request.Email) ||
                !request.Email.Contains("@"))
            {
                throw new ArgumentException("A valid email address is required.");
            }

            if (await _ReadcustomerRepo.EmailExists(request.Email))
                throw new ArgumentException("Email is already registered.");

            var customer = new Customer
            {
                FullName = request.FullName,
                Email = request.Email,
                IsVip = request.IsVip
            };

            await _customerRepo.Add(customer);
            await _customerRepo.SaveChanges();

            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                IsVip = customer.IsVip
            };
        }
    }
}