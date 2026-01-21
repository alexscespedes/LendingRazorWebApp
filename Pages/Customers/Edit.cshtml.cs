using System.Net;
using System.Text;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public Customer? Customer { get; set; }
        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Customers/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Customer = await JsonSerializer.DeserializeAsync<Customer>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var httpClient = CreateClient();

            if (id == null)
                return NotFound();

            var customer = new StringContent(JsonSerializer.Serialize(Customer), Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PutAsync($"Customers/{id}", customer);

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Error updating a customer");
            return Page();
        }
    }
}
