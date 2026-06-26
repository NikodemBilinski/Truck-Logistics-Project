namespace TrucksLogisticsServerAPI.Models.Helping_Models
{
    public class UsersResponse
    {
        public List<Users> Users { get; set; } = new();
        public int Users_Count { get; set; }

        public int AvaiableUsers_Count { get; set; }

        public int BusyUsers_Count { get; set; }

        public int Admin_Count { get; set; }

        public int User_Count { get; set; }
    }
}
