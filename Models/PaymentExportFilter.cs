using System;

namespace LendingRazorWeb.Models;

public class PaymentExportFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int? LoanId { get; set; }
    public int? UserId { get; set; }
    public List<int>? CustomerIds { get; set; }
}
