using System;
using System.ComponentModel.DataAnnotations;
using LendingRazorWeb.Enums;

namespace LendingRazorWeb.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public Role Role { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
