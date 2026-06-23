namespace EventManagement.API.DTO
{
    public class LoginResponseDto
    {
        public string Token { get; set; }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }
    }
}
