
using CommunityHealthServiceTracker.Models;
using CommunityHealthServiceTracker.Pages.Referrals;
using CommunityHealthServiceTracker.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityHealthServiceTracker.Tests;

public class ReferralDeleteTests
{
    // TEST 1: GET loads the correct referral for confirmation.
    [Fact]
    public async Task OnGetAsync_WithValidId_LoadsReferral()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var referral = CreateTestReferral("DELETE-001");
        context.Referrals.Add(referral);
        await context.SaveChangesAsync();

        var pageModel = new DeleteModel(context);

        var result = await pageModel.OnGetAsync(referral.Id);

        Assert.IsType<PageResult>(result);
        Assert.Equal(referral.Id, pageModel.Referral.Id);
        Assert.Equal("DELETE-001", pageModel.Referral.Reference);
        Assert.Equal(referral.ServiceName, pageModel.Referral.ServiceName);
        Assert.Equal(referral.Summary, pageModel.Referral.Summary);

        // Viewing the confirmation page must not delete the record.
        Assert.Equal(1, await context.Referrals.CountAsync());
    }

    // TEST 2: GET without an ID returns NotFound.
    [Fact]
    public async Task OnGetAsync_WithNullId_ReturnsNotFound()
    {
        await using var database = await TestDatabase.CreateAsync();
        var pageModel = new DeleteModel(database.Context);

        var result = await pageModel.OnGetAsync(null);

        Assert.IsType<NotFoundResult>(result);
    }

    // TEST 3: GET with an unknown ID returns NotFound.
    [Fact]
    public async Task OnGetAsync_WithNonexistentId_ReturnsNotFound()
    {
        await using var database = await TestDatabase.CreateAsync();
        var pageModel = new DeleteModel(database.Context);

        var result = await pageModel.OnGetAsync(99999);

        Assert.IsType<NotFoundResult>(result);
    }

    // TEST 4: POST deletes the selected referral and redirects.
    [Fact]
    public async Task OnPostAsync_WithValidId_DeletesReferralAndRedirects()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var referral = CreateTestReferral("DELETE-004");
        context.Referrals.Add(referral);
        await context.SaveChangesAsync();

        var pageModel = new DeleteModel(context);

        var result = await pageModel.OnPostAsync(referral.Id);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirect.PageName);

        context.ChangeTracker.Clear();

        Assert.Empty(await context.Referrals.AsNoTracking().ToListAsync());
    }

    // TEST 5: POST without an ID does not delete anything.
    [Fact]
    public async Task OnPostAsync_WithNullId_ReturnsNotFoundAndDoesNotDelete()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        context.Referrals.Add(CreateTestReferral("DELETE-005"));
        await context.SaveChangesAsync();

        var pageModel = new DeleteModel(context);

        var result = await pageModel.OnPostAsync(null);

        Assert.IsType<NotFoundResult>(result);
        Assert.Equal(1, await context.Referrals.CountAsync());
    }

    // TEST 6: POST with an unknown ID does not delete anything.
    [Fact]
    public async Task OnPostAsync_WithNonexistentId_ReturnsNotFoundAndDoesNotDelete()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        context.Referrals.Add(CreateTestReferral("DELETE-006"));
        await context.SaveChangesAsync();

        var pageModel = new DeleteModel(context);

        var result = await pageModel.OnPostAsync(99999);

        Assert.IsType<NotFoundResult>(result);
        Assert.Equal(1, await context.Referrals.CountAsync());
    }

    // TEST 7: Deleting one referral must not delete another.
    [Fact]
    public async Task OnPostAsync_DeletesOnlyTheSelectedReferral()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var firstReferral = CreateTestReferral("DELETE-007-A");
        var secondReferral = CreateTestReferral("DELETE-007-B");

        context.Referrals.AddRange(firstReferral, secondReferral);
        await context.SaveChangesAsync();

        var firstId = firstReferral.Id;
        var secondId = secondReferral.Id;

        var pageModel = new DeleteModel(context);

        var result = await pageModel.OnPostAsync(firstId);

        Assert.IsType<RedirectToPageResult>(result);

        context.ChangeTracker.Clear();

        var remainingReferrals = await context.Referrals
            .AsNoTracking()
            .ToListAsync();

        var remaining = Assert.Single(remainingReferrals);

        Assert.Equal(secondId, remaining.Id);
        Assert.Equal("DELETE-007-B", remaining.Reference);

        Assert.False(
            await context.Referrals.AnyAsync(r => r.Id == firstId));
    }

    // Reusable fictional referral for test setup.
    private static Referral CreateTestReferral(string reference)
    {
        return new Referral
        {
            Reference = reference,
            ServiceName = "Community Physiotherapy",
            Status = ReferralStatus.New,
            Priority = ReferralPriority.Routine,
            ReferralDate = new DateTime(2026, 10, 8),
            Summary = "Fictional referral for delete testing",
            AssignedTeam = "North Community Team"
        };
    }
}
