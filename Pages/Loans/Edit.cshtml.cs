using System.Net;
using System.Text;
using System.Text.Json;
using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LendingRazorWeb.Pages.Loans
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public Loan? Loan { get; set; }

        public SelectList CustomerList { get; set; } = default!;
        private HttpClient CreateClient() => _httpClientFactory.CreateClient("LendingWebApi");


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var httpClient = CreateClient();
            var httpResponseMessage = await httpClient.GetAsync($"Loans/{id}");

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            Loan = await JsonSerializer.DeserializeAsync<Loan>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var httpResponseCustomers = await httpClient.GetAsync($"Customers");
            using var contentCustomersStream = await httpResponseCustomers.Content.ReadAsStreamAsync();

            var customers = await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(contentCustomersStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            CustomerList = new SelectList(customers, "Id", "FullName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var httpClient = CreateClient();

            if (id == null)
                return NotFound();

            var loan = new StringContent(JsonSerializer.Serialize(Loan), Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PutAsync($"Loans/{id}", loan);

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (httpResponseMessage.IsSuccessStatusCode)
                return RedirectToPage("Index");

            ModelState.AddModelError(string.Empty, "Error updating a loan");
            return Page();
        }
    }
}
