using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using TrucksLogisticsClient.Models;

namespace TrucksLogisticsClient.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string brand { get; set; } = string.Empty;

        public bool IsBusy { get; set; }

        public int Capacity { get; set; }

        [JsonIgnore]
        public List<Users> AssignedUsers { get; set; } = new();

        [JsonIgnore]
        public Color? SelectionColor { get; set; }


    }
}
