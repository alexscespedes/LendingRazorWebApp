using System.Text;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Payments
{
    public class ExportModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImportModel> _logger;

        public ExportModel(
            IHttpClientFactory httpClientFactory,
            ILogger<ImportModel> logger)
        {
             _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public PaymentExportFilter Filter { get; set; } = new();

        [BindProperty]
        public string? CustomerIdsInput { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string exportType)
        {
            if (!string.IsNullOrWhiteSpace(CustomerIdsInput))
            {
                Filter.CustomerIds = CustomerIdsInput
                    .Split(',')
                    .Select(id => id.Trim())
                    .Where(id => int.TryParse(id, out _))
                    .Select(int.Parse)
                    .ToList();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("LendingWebApi");

                var jsonContent = JsonSerializer.Serialize(Filter);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var endpoint = exportType == "streaming"
                    ? "PaymentExport/export/streaming"
                    : "PaymentExport/export";

                var response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    var fileName = $"payments_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                    return File(fileBytes, "text/csv", fileName);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Export failed: {Error}", errorContent);
                    TempData["ErrorMessage"] = "Export failed. Please try again.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during export");
                TempData["ErrorMessage"] = "An error occurred during export. Please try again.";
                return Page();
            }
        }
    }
}
