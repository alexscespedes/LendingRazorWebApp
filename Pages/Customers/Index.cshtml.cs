using System.Net;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IEnumerable<Customer>? CustomersList { get; set; }
        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnGetAsync()
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Customers");

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            CustomersList = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }
    }
}
