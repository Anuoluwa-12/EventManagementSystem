namespace EventManagement.API.Entity
{
    public class Company
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string CompanyEmail { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string? Website { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
