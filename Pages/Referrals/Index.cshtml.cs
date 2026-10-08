using CommunityHealthServiceTracker.Data;
using CommunityHealthServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityHealthServiceTracker.Pages.Referrals;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public ReferralStatus? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public ReferralPriority? Priority { get; set; }

    public IList<Referral> Referrals { get; set; } = new List<Referral>();

    public async Task OnGetAsync()
    {
        var query = _context.Referrals.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm.Trim();

            query = query.Where(r =>
                r.Reference.Contains(term) ||
                r.ServiceName.Contains(term) ||
                (r.AssignedTeam != null && r.AssignedTeam.Contains(term)));
        }

        if (Status.HasValue)
        {
            query = query.Where(r => r.Status == Status.Value);
        }

        if (Priority.HasValue)
        {
            query = query.Where(r => r.Priority == Priority.Value);
        }

        Referrals = await query
            .OrderByDescending(r => r.ReferralDate)
            .ThenByDescending(r => r.Id)
            .ToListAsync();
    }
}
