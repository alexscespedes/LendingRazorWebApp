using System.Net;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Customers
{
    public class DetailModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailModel(IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory;
        }

        public Customer? Customer {get; set; }
        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Customers/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();
                

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);


            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Customer = await JsonSerializer.DeserializeAsync<Customer>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }
    }
}
