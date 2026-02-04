using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Payments
{
    public class ImportModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImportModel> _logger;

        public ImportModel(
            IHttpClientFactory httpClientFactory,
            ILogger<ImportModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public BulkImportResult? ImportResult { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return Page();
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Only CSV files are allowed.";
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("LendingWebApi");

                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                content.Add(streamContent, "file", file.FileName);

                var response = await client.PostAsync("PaymentBulk/import", content);

                if (response.IsSuccessStatusCode)
                {
                    ImportResult = await response.Content.ReadFromJsonAsync<BulkImportResult>();

                    if (ImportResult != null)
                    {
                        if (ImportResult.FailedRecords == 0)
                        {
                            TempData["SuccessMessage"] = $"Sucessfully imported {ImportResult.SuccessfulRecords} payments.";

                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Import completed with {ImportResult?.FailedRecords}. Successfully imported {ImportResult?.SuccessfulRecords} payments.";
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Import failed: {Error}", errorContent);
                    TempData["ErrorMessage"] = "Import failed. Please check the file format and try again.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import");
                TempData["ErrorMessage"] = "An error occurred during import. Please try again.";
            }

            return Page();
        }
    }
}
