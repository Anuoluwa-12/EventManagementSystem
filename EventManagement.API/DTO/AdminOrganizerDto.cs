namespace EventManagement.API.DTO
{
public class AdminOrganizerDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string DisplayName { get; set; } =
        string.Empty;

    public string Email { get; set; } =
        string.Empty;

    public string? Phone { get; set; }

    public string? Bio { get; set; }

    public string Status { get; set; } =
        string.Empty;

    public int TotalEvents { get; set; }

    public DateTime CreatedAt { get; set; }
}


}
