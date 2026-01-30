using System.Net;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Loans
{
    public class DetailModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailModel(IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory;
        }

        public Loan? Loan { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("LendingWebApi");
            var httpResponseMessage = await httpClient.GetAsync($"Loans/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();
                

            if (!httpResponseMessage.IsSuccessStatusCode)
                return StatusCode((int)httpResponseMessage.StatusCode);

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Loan = await JsonSerializer.DeserializeAsync<Loan>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Page();
        }
    }
}
