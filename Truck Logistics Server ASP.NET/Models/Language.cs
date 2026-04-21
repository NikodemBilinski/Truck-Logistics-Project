using System.Text.Json.Serialization;

namespace TrucksLogisticsServerAPI.Models
{
    public class Language
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Users> Users { get; set; } = new();
    }
}
