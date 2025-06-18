using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using RoomReservationSystem.Models;

public class RoomsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RoomsModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<Room> Rooms { get; set; }

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient();

        if (TempData["JwtToken"] != null)
        {
            var token = TempData["JwtToken"].ToString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.GetAsync("http://localhost:5196/api/room");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Rooms = JsonSerializer.Deserialize<List<Room>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else
        {
            Rooms = new List<Room>();
        }
    }
}
