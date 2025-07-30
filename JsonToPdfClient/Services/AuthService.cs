using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JsonToPdfClient.Utils;      

namespace JsonToPdfClient.Services
{
    public static class AuthService
    {
        private static readonly HttpClient _client;

        // Guarda aquí el JWT tras el login
        public static string JwtToken { get; private set; }

        static AuthService()
        {
            // Construye el cliente apuntando a /api/Auth/
            _client = new HttpClient
            {
                BaseAddress = new Uri($"{ClientConfig.ApiBaseUrl}/Auth/")
            };
        }

        /// <summary>
        /// Hace POST a api/Auth/login, guarda el JWT y configura el header.
        /// </summary>
        public static async Task LoginAsync(string email, string password)
        {
            var payload = new { Email = email, Password = password };
            var content = new StringContent(
                JObject.FromObject(payload).ToString(),
                Encoding.UTF8, "application/json");

            var resp = await _client.PostAsync("login", content);
            resp.EnsureSuccessStatusCode();

            var body = await resp.Content.ReadAsStringAsync();
            var obj = JObject.Parse(body);

            JwtToken = (string)obj["Token"]
                ?? throw new Exception("No se recibió token.");

            // Configura el bearer header en el mismo cliente
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtToken);
        }

        /// <summary>
        /// Exponemos el HttpClient ya con el Authorization header
        /// para consumir cualquier endpoint protegido.
        /// </summary>
        public static HttpClient HttpClient => _client;
    }
}
