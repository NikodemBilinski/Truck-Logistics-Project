using System;
using System.Collections.Generic;
using System.Text;

namespace TrucksLogisticsClient.Models
{
    public class Invoice
    {
        public int ID { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }

        public decimal NetAmount { get; set; }
        public int VatRate { get; set; } = 23;

        public decimal GrossAmount { get; set; }

        public int ClientID { get; set; }
        public Client? Client { get; set; }

        public int JobID { get; set; }
        public Job? Job { get; set; }
        public string Status { get; set; } = "unpaid";
    }
}
