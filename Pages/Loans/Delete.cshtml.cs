using System.Net;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Loans
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public Loan? Loan { get; set; }
        public string? ErrorMessage { get; set; }
        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnGetAsync(int id, bool? saveChangesError = false)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Loans/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (saveChangesError.GetValueOrDefault())
                ErrorMessage = string.Format("Delete {ID} failed. Try again", id);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Loan = await JsonSerializer.DeserializeAsync<Loan>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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

            var httpResponseMessage = await httpClient.DeleteAsync($"Loans/{id}");

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ErrorMessage = "Failed to delete loan. Please try again";
            return Page();
        }
    }
}
