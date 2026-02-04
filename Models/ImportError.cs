using System;

namespace LendingRazorWeb.Models;

public class ImportError
{
    public int RowNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string? FieldName { get; set; }
}
