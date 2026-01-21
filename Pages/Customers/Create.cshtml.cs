using System.Text;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public Customer? Customer { get; set; }
        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var customer = new StringContent(JsonSerializer.Serialize(Customer), Encoding.UTF8, "application/json");

            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.PostAsync("Customers", customer);

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Error creating a customer");
            return Page();
        }
    }
}
