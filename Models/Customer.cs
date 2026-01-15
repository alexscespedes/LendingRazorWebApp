using System;
using System.ComponentModel.DataAnnotations;

namespace LendingRazorWeb.Models;

public class Customer
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
