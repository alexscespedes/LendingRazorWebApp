using LendingRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LendingRazorWeb.Pages.User
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginRequest? Input { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var client = _httpClientFactory.CreateClient("LendingWebApi");

            var response = await client.PostAsJsonAsync("Auth/Login", Input);

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                HttpContext.Session.SetString("JWTToken", authResponse!.Token!);
                HttpContext.Session.SetString("Username", authResponse!.Username!);

                return RedirectToPage("../Index");
            }

            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}
