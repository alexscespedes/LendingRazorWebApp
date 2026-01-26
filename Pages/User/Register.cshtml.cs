using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.User
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public RegisterRequest? Input { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            
            var client = _httpClientFactory.CreateClient("LendingWebApi");

            var response = await client.PostAsJsonAsync("Auth/Register", Input);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("../Index");

            var error = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Registration failed: {error}";

            return Page();
        }
    }
}
