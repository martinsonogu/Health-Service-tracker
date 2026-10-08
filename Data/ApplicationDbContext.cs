using CommunityHealthServiceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityHealthServiceTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Referral> Referrals { get; set; } = default!;
}