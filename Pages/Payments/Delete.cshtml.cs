using System.Net;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Payments
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Payment? Payment { get; set; }
        public string? ErrorMessage { get; set; }

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnGetAsync(int id, bool? saveChangesError = false)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Payments/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (saveChangesError.GetValueOrDefault())
                ErrorMessage = string.Format("Delete {ID} failed. Try again", id);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Payment = await JsonSerializer.DeserializeAsync<Payment>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var httpClient = CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            if (id == null)
                return NotFound();

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var httpResponseMessage = await httpClient.DeleteAsync($"Payments/{id}");

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ErrorMessage = "Failed to delete payment. Please try again";
            return Page();
        }
    }
}
