using System.Text;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LendingRazorWeb.Pages.Loans
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [BindProperty]
        public Loan? Loan { get; set; }

        public SelectList CustomerList { get; set; } = default!;

        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");

        public async Task<IActionResult> OnGetAsync()
        {
            var httpClient = CreateClient();

            var httpResponseCustomers = await httpClient.GetAsync($"Customers");
            using var contentCustomersStream = await httpResponseCustomers.Content.ReadAsStreamAsync();

            var customers = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(contentCustomersStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            CustomerList = new SelectList(customers, "Id", "FullName");

            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            var httpClient = CreateClient();

            if (!ModelState.IsValid)
            {
                var httpResponseCustomers = await httpClient.GetAsync($"Customers");
                using var contentCustomersStream = await httpResponseCustomers.Content.ReadAsStreamAsync();

                var customers = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(contentCustomersStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                CustomerList = new SelectList(customers, "Id", "FullName");

                return Page();
            }

            var loan = new StringContent(JsonSerializer.Serialize(Loan), Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PostAsync($"Loans", loan);

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Error creating a customer");
            return Page();
        }
    }
}
