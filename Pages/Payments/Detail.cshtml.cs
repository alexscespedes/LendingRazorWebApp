using System.Net;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Payments
{
    public class DetailModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailModel(IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory;
        }

        public Payment? Payment { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("LendingWebApi");
            var httpResponseMessage = await httpClient.GetAsync($"Payments/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Payment = await JsonSerializer.DeserializeAsync<Payment>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }
    }
}
