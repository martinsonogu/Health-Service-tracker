using CommunityHealthServiceTracker.Data;
using CommunityHealthServiceTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommunityHealthServiceTracker.Pages.Referrals;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Referral Referral { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Referrals.Add(Referral);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}