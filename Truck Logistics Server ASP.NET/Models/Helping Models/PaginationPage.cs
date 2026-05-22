namespace TrucksLogisticsServerAPI.Models.Helping_Models
{
    public class PaginationPage
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }
    }
}
