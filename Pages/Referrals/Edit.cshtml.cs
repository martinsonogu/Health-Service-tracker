
using CommunityHealthServiceTracker.Data;
using CommunityHealthServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityHealthServiceTracker.Pages.Referrals;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public EditReferralInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var referral = await _context.Referrals
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id.Value);

        if (referral is null)
        {
            return NotFound();
        }

        Input = new EditReferralInput
        {
            Id = referral.Id,
            Reference = referral.Reference,
            ServiceName = referral.ServiceName,
            Status = referral.Status,
            Priority = referral.Priority,
            ReferralDate = referral.ReferralDate,
            Summary = referral.Summary,
            AssignedTeam = referral.AssignedTeam
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var referral = await _context.Referrals
            .FindAsync(Input.Id);

        if (referral is null)
        {
            return NotFound();
        }

        referral.Reference = Input.Reference;
        referral.ServiceName = Input.ServiceName;
        referral.Status = Input.Status;
        referral.Priority = Input.Priority;
        referral.ReferralDate = Input.ReferralDate;
        referral.Summary = Input.Summary;
        referral.AssignedTeam = Input.AssignedTeam;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
