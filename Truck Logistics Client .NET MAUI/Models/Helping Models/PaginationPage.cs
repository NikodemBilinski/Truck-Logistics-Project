using System;
using System.Collections.Generic;
using System.Text;

namespace TrucksLogisticsClient.Models.Helping_Models
{
    public class PaginationPage
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }
    }
}
