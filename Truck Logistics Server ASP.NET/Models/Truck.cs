namespace TrucksLogisticsServerAPI.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public string Owner { get; set; } = string.Empty;

        public bool IsBusy { get; set; }

        public int Capacity { get; set; }

    }
}
