using Microsoft.AspNetCore.Mvc.RazorPages;
using CommunityHealthServiceTracker.Models;
using CommunityHealthServiceTracker.Pages.Referrals;
using CommunityHealthServiceTracker.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityHealthServiceTracker.Tests;

public class ReferralCreateTests
{
    [Fact]
    public async Task OnPostAsync_WithValidReferral_SavesReferralAndRedirects()
    {
        // Arrange: Create an isolated SQLite test database.
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var pageModel = new CreateModel(context)
        {
            Referral = new Referral
            {
                Reference = "TEST-CREATE-001",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional referral created during testing",
                Status = ReferralStatus.New,
                Priority = ReferralPriority.Routine,
                ReferralDate = new DateTime(2026, 10, 8),
                AssignedTeam = "North Community Team"
            }
        };

        // Act: Submit the referral.
        var result = await pageModel.OnPostAsync();

        // Assert: The user should be redirected to the Referrals page.
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirect.PageName);

        // Assert: The referral should exist in the database.
        var savedReferral = await context.Referrals
            .AsNoTracking()
            .SingleAsync();

        Assert.Equal("TEST-CREATE-001", savedReferral.Reference);
        Assert.Equal("Community Physiotherapy", savedReferral.ServiceName);
        Assert.Equal(ReferralStatus.New, savedReferral.Status);
        Assert.Equal(ReferralPriority.Routine, savedReferral.Priority);
        Assert.Equal("North Community Team", savedReferral.AssignedTeam);
    }
        [Fact]
    public async Task OnPostAsync_WithInvalidModelState_DoesNotSaveReferral()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var pageModel = new CreateModel(context)
        {
            Referral = new Referral
            {
                Reference = "",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional test referral"
            }
        };

        // Simulate a validation error.
        pageModel.ModelState.AddModelError(
            "Referral.Reference",
            "The Reference field is required.");

        // Act
        var result = await pageModel.OnPostAsync();

        // Assert: The form should be displayed again.
        Assert.IsType<PageResult>(result);

        // Assert: Nothing should be saved.
        Assert.Empty(await context.Referrals.ToListAsync());
    }
}
