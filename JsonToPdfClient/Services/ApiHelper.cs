using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml.Linq;

namespace JsonToPdfClient.Services
{
    static class ApiHelper
    {
        public static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage resp)
        {
            var text = await resp.Content.ReadAsStringAsync();
            try
            {
                var obj = JObject.Parse(text);
                if (obj["Mensaje"] != null) return (string)obj["Mensaje"];
                if (obj["message"] != null) return (string)obj["message"];
            }
            catch { }
            return text;
        }
    }
}
