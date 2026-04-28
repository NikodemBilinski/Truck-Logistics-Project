namespace TrucksLogisticsServerAPI.Models
{
    public class Job
    {
        // na pewno dodac firma zleceniowa, kontakt do klienta (numer), assigned user 
        public int ID { get; set; }
        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string ClientContactNumber { get; set; }

        public DateTime Created { get; set; }

        public DateTime DeadLine { get; set; }

        public string LocationFrom { get; set; }

        public string LocationTo { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        //Requierments

        public string RequiredLanguages { get; set; } = string.Empty;

        public int RequiredMinimumCapacity { get; set; }

        public string RequiredTruckBrand { get; set; } = string.Empty;

        // assigned user

        public int? AssignedUserId { get; set; }
    }
}
