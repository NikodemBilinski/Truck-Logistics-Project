namespace TrucksLogisticsServerAPI.Models
{
    public class Users
    {

        // login data
        public int ID { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        //profile data

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int Age { get; set; }

        public List<Truck> AssignedTrucks { get; set; } = new();

        public List<Job> AssignedJobs { get; set; } = new();

        public bool isBusy { get; set; } = false;

        public List<Language> Languages { get; set; } = new();


    }
}
