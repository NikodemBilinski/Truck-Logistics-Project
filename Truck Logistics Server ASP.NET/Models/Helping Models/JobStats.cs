using System;
using System.Collections.Generic;
using System.Text;

namespace TrucksLogisticsClient.Models.Helping_Models
{
    public class JobStats
    {
        public int Jobs_Count { get; set; }
        public int Open_Count { get; set; }

        public int NearDeadline_Count { get; set; }

        public int Assigned_Count { get; set; }

        public int Delivered_Count { get; set; }
    }
}
