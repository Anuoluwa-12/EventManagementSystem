using EventManagement.API.Entity;

namespace EventManagement.API.Models;

public class OrganizerProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Bio { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
}