using System.ComponentModel.DataAnnotations;

namespace EventManagement.API.DTO;

public class RegisterOrganizerRequestDto
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Bio { get; set; }
}


