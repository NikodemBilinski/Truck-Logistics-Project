namespace TrucksLogisticsServerAPI.Models.Helping_Models
{
    public class InvoicesStats
    {
        public int Invoices_Count { get; set; }
        public int Unpaid_Count { get; set; }
        public int Overdue_Count { get; set; }
    }
}
