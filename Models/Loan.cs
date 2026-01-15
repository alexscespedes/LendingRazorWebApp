using System;
using System.ComponentModel.DataAnnotations;
using LendingRazorWeb.Enums;

namespace LendingRazorWeb.Models;

public class Loan
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermsMonth { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    public LoanStatus LoanStatus { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
