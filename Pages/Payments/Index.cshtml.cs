using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Payments
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IEnumerable<Payment>? PaymentsList { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("LendingWebApi");
            var httpResponseMessage = await httpClient.GetAsync($"Payments");
            
            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            PaymentsList = await JsonSerializer.DeserializeAsync<IEnumerable<Payment>>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return Page();
        }
    }
}
