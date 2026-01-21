using System.Net;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Customers
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Customer? Customer { get; set; }
        public string? ErrorMessage { get; set; }
        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnGetAsync(int id, bool? saveChangesError = false)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Customers/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (saveChangesError.GetValueOrDefault())
                ErrorMessage = string.Format("Delete {ID} failed. Try again", id);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Customer = await JsonSerializer.DeserializeAsync<Customer>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("Go to delete page!");
            return Page();
        }

         public async Task<IActionResult> OnPostAsync(int? id)
        {
            var httpClient = CreateClient();

            if (id == null)
                return NotFound();

            var httpResponseMessage = await httpClient.DeleteAsync($"Customers/{id}");

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ErrorMessage = "Failed to delete customer. Please try again";
            return Page();
        }
    }
}
