using System;
using System.Collections.Generic;
using System.Text;
using TrucksLogisticsServerAPI.Models;

namespace TrucksLogisticsClient.Models.Helping_Models
{
    public class TrucksResponse
    {
        public List<Truck> Trucks { get; set; } = new();
        public int Truck_Count { get; set; }

        public int AvaiableTrucks_Count { get; set; }

        public int BusyTrucks_Count { get; set; }

        public int DiffrentBrands_Count { get; set; }
    }
}
