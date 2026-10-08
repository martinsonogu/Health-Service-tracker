using CommunityHealthServiceTracker.Data;
using CommunityHealthServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityHealthServiceTracker.Pages.Referrals;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Referral Referral { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var referral = await _context.Referrals
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (referral is null)
        {
            return NotFound();
        }

        Referral = referral;

        return Page();
    }
}