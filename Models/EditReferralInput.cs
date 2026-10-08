using System.ComponentModel.DataAnnotations;

namespace CommunityHealthServiceTracker.Models;

public class EditReferralInput
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Reference { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ServiceName { get; set; } = string.Empty;

    public ReferralStatus Status { get; set; }

    public ReferralPriority Priority { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReferralDate { get; set; }

    [Required]
    [StringLength(500)]
    public string Summary { get; set; } = string.Empty;

    [StringLength(100)]
    public string? AssignedTeam { get; set; }
}