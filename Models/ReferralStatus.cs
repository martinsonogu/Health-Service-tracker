using System.ComponentModel.DataAnnotations;

namespace CommunityHealthServiceTracker.Models;

public enum ReferralStatus
{
    New,

    [Display(Name = "In Progress")]
    InProgress,

    Completed
}