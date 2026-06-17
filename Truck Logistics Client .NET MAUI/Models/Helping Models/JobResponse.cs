using System;
using System.Collections.Generic;
using System.Text;

namespace TrucksLogisticsClient.Models.Helping_Models
{
    public class JobResponse
    {
        public List<Job> Jobs { get; set; } = new();
        public int Jobs_Count { get; set; }
        public int Open_Count { get; set; }

        public int NearDeadline_Count { get; set; }

        public int Assigned_Count { get; set; }

        public int Delivered_Count { get; set; }
    }
}
