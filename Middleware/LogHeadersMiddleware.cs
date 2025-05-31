using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RoomReservationSystem.Middleware
{
    public class LogHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public LogHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("---- Request Headers ----");

            foreach (var header in context.Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            Console.WriteLine("-------------------------");

            await _next(context);
        }
    }
}
