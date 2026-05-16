using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Authentication;

namespace TrucksLogisticsClient.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public Users? User { get; set; }
    }
}
