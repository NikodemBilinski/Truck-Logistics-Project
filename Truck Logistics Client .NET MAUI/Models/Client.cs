using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TrucksLogisticsClient.Models
{
    public class Client
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string NIP { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }


        //relacje 
        [JsonIgnore]
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
        [JsonIgnore]
        public List<Job> Jobs { get; set; } = new List<Job>();

    }
}
