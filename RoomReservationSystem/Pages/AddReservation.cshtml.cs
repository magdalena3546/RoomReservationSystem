using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using RoomReservationSystem.Models;

namespace RoomReservationSystem.Pages
{
    public class AddReservationModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AddReservationModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public int RoomId { get; set; }

        [BindProperty]
        public int GuestId { get; set; }

        [BindProperty]
        public DateTime StartTime { get; set; }

        [BindProperty]
        public DateTime EndTime { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7016");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var reservation = new Reservation
            {
                RoomId = RoomId,
                GuestId = GuestId,
                StartTime = StartTime,
                EndTime = EndTime
            };

            var response = await client.PostAsJsonAsync("/api/Reservation", reservation);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Reservations");
            }
            else
            {
                ErrorMessage = $"Failed to add reservation. Status code: {response.StatusCode}";
                return Page();
            }
        }
    }
}
