namespace EventManagement.API.Entity
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }
        public string? AccountType { get; set; }

        public int? CompanyId { get; set; }

        public Company? Company { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ResetPasswordToken { get; set; }

        public DateTime? ResetPasswordTokenExpiry { get; set; }
    }
}