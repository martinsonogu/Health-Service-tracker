using CommunityHealthServiceTracker.Data;
using CommunityHealthServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityHealthServiceTracker.Pages.Referrals;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var referral = await _context.Referrals.FindAsync(id);

        if (referral is null)
        {
            return NotFound();
        }

        _context.Referrals.Remove(referral);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}