using System.ComponentModel.DataAnnotations;

namespace CommunityHealthServiceTracker.Models;

public class Referral
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Referral Reference")]
    public string Reference { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Service")]
    public string ServiceName { get; set; } = string.Empty;

    public ReferralStatus Status { get; set; } = ReferralStatus.New;

    public ReferralPriority Priority { get; set; } = ReferralPriority.Routine;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Referral Date")]
    public DateTime ReferralDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(500)]
    public string Summary { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Assigned Team")]
    public string? AssignedTeam { get; set; }
}