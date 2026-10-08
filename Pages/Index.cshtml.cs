using CommunityHealthServiceTracker.Data;
using CommunityHealthServiceTracker.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityHealthServiceTracker.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalReferrals { get; set; }
    public int NewReferrals { get; set; }
    public int InProgressReferrals { get; set; }
    public int CompletedReferrals { get; set; }
    public int UrgentReferrals { get; set; }

    public IList<Referral> RecentReferrals { get; set; }
        = new List<Referral>();

    public async Task OnGetAsync()
    {
        TotalReferrals = await _context.Referrals.CountAsync();

        NewReferrals = await _context.Referrals
            .CountAsync(r => r.Status == ReferralStatus.New);

        InProgressReferrals = await _context.Referrals
            .CountAsync(r => r.Status == ReferralStatus.InProgress);

        CompletedReferrals = await _context.Referrals
            .CountAsync(r => r.Status == ReferralStatus.Completed);

        UrgentReferrals = await _context.Referrals
            .CountAsync(r => r.Priority == ReferralPriority.Urgent);

        RecentReferrals = await _context.Referrals
            .AsNoTracking()
            .OrderByDescending(r => r.ReferralDate)
            .ThenByDescending(r => r.Id)
            .Take(5)
            .ToListAsync();
    }
}