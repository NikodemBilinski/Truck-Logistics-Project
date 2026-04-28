using System;
using System.Collections.Generic;
using System.Text;

namespace TrucksLogisticsClient.Services
{
    public class ApiService
    {
        private readonly HttpClient client;
        private string apiUrl = "http://192.168.0.218:5160/api/Values/";
    }
}
