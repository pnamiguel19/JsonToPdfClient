using System;
using System.Configuration;

namespace JsonToPdfClient.Utils
{
    public static class ClientConfig
    {
        public static string ApiBaseUrl =>
            ConfigurationManager.AppSettings["ApiBaseUrl"]
            ?? throw new InvalidOperationException("ApiBaseUrl no configurada");
    }
}

