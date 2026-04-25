using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TrucksLogisticsClient.Models
{
    public class Language
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Users> Users { get; set; } = new();

        [JsonIgnore]
        public Color? SelectionColor { get; set; }

    }
}
