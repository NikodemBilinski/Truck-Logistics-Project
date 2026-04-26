using System;
using System.Collections.Generic;
using System.Text;

namespace TrucksLogisticsClient.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime DeadLine { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        //Requierments

        public string RequiredLanguages { get; set; } = string.Empty;

        public int RequiredMinimumCapacity { get; set; }

        public string RequiredTruckBrand { get; set; } = string.Empty;
    }
}
