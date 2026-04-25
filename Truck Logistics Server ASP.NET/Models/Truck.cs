using System.Text.Json.Serialization;
namespace TrucksLogisticsServerAPI.Models
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

    }
}
