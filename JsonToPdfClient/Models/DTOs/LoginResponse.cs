using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToPdfClient.Models.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
