using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RoomReservationSystem.Models;

namespace RoomReservationSystem.Pages
{
    public class ReservationsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReservationsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<Reservation> Reservations { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7016");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/Reservation");

            if (response.IsSuccessStatusCode)
            {
                var reservations = await response.Content.ReadFromJsonAsync<List<Reservation>>();
                if (reservations != null)
                {
                    Reservations = reservations;
                }
            }
            else
            {
            }
               

            return Page();
        }
    }
}
