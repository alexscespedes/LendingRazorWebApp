using System;

namespace LendingRazorWeb.Models;

public class BulkImportResult
{
    public int TotalRecords { get; set; }
    public int SuccessfulRecords { get; set; }
    public int FailedRecords { get; set; }
    public List<ImportError> Errors { get; set; } = new();
}
