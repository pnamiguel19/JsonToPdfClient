using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JsonToPdfClient.Models.DTOs;
using Newtonsoft.Json;

namespace TuProyecto.Services
{
    public static class AuthService
    {
        private static readonly HttpClient _client =
            new HttpClient { BaseAddress = new Uri("https://localhost:44369/api/Auth/") };

        public static string JwtToken { get; private set; }

        public static async Task LoginAsync(string email, string password)
        {
            var req = new LoginRequest { Email = email, Password = password };
            var json = JsonConvert.SerializeObject(req);
            var resp = await _client.PostAsync("login",
                new StringContent(json, Encoding.UTF8, "application/json"));

            resp.EnsureSuccessStatusCode();

            var body = await resp.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponse>(body);

            JwtToken = result.Token;
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtToken);
        }
    }
}
