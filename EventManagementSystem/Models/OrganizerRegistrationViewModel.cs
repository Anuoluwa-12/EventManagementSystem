using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models;

public class OrganizerRegistrationViewModel
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Bio { get; set; }
}
