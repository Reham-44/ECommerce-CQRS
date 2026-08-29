using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsVip { get; set; }
    public List<CustomerOrderDto> Orders { get; set; } = new();
}

public class CustomerOrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
}