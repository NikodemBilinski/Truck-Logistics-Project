namespace TrucksLogisticsClient.Models.Helping_Models
{
    public class ClientsStats
    {
        public List<Client> Clients { get; set; } = new();
        public int TotalClients { get; set; }
    }
}
