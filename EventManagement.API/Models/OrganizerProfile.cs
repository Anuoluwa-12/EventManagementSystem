using EventManagement.API.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.API.Models;

[Table("OrganizerProfile", Schema = "dbo")]
public class OrganizerProfile
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("bio")]
    public string? Bio { get; set; }

    [Required]
    [Column("status")]
    public string Status { get; set; } = "Active";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}