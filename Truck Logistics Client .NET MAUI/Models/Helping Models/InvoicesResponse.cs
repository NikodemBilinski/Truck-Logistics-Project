using System;
using System.Collections.Generic;
using System.Text;

namespace TrucksLogisticsClient.Models.Helping_Models
{
    public class InvoicesResponse
    {
        public List<Invoice> Invoices { get; set; } = new();
        public int Invoices_Count { get; set; }
        public int Unpaid_Count { get; set; }
        public int Overdue_Count { get; set; }
    }
}
