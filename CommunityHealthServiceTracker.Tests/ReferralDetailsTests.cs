
using CommunityHealthServiceTracker.Models;
using CommunityHealthServiceTracker.Pages.Referrals;
using CommunityHealthServiceTracker.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityHealthServiceTracker.Tests;

public class ReferralDetailsTests
{
    // TEST 1: A valid ID loads the correct referral.
    [Fact]
    public async Task OnGetAsync_WithValidId_LoadsReferral()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var referral = CreateTestReferral("DETAIL-001");

        context.Referrals.Add(referral);
        await context.SaveChangesAsync();

        var pageModel = new DetailsModel(context);

        var result = await pageModel.OnGetAsync(referral.Id);

        Assert.IsType<PageResult>(result);
        Assert.Equal(referral.Id, pageModel.Referral.Id);
        Assert.Equal("DETAIL-001", pageModel.Referral.Reference);
        Assert.Equal(referral.ServiceName, pageModel.Referral.ServiceName);
        Assert.Equal(referral.Status, pageModel.Referral.Status);
        Assert.Equal(referral.Priority, pageModel.Referral.Priority);
        Assert.Equal(referral.ReferralDate, pageModel.Referral.ReferralDate);
        Assert.Equal(referral.Summary, pageModel.Referral.Summary);
        Assert.Equal(referral.AssignedTeam, pageModel.Referral.AssignedTeam);
    }

    // TEST 2: A missing ID returns NotFound.
    [Fact]
    public async Task OnGetAsync_WithNullId_ReturnsNotFound()
    {
        await using var database = await TestDatabase.CreateAsync();

        var pageModel = new DetailsModel(database.Context);

        var result = await pageModel.OnGetAsync(null);

        Assert.IsType<NotFoundResult>(result);
    }

    // TEST 3: An unknown ID returns NotFound.
    [Fact]
    public async Task OnGetAsync_WithNonexistentId_ReturnsNotFound()
    {
        await using var database = await TestDatabase.CreateAsync();

        var pageModel = new DetailsModel(database.Context);

        var result = await pageModel.OnGetAsync(99999);

        Assert.IsType<NotFoundResult>(result);
    }

    // TEST 4: Opening one referral loads the correct record.
    [Fact]
    public async Task OnGetAsync_WithMultipleReferrals_LoadsOnlySelectedReferral()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var firstReferral = CreateTestReferral("DETAIL-004-A");
        var secondReferral = CreateTestReferral("DETAIL-004-B");

        context.Referrals.AddRange(firstReferral, secondReferral);
        await context.SaveChangesAsync();

        var pageModel = new DetailsModel(context);

        var result = await pageModel.OnGetAsync(secondReferral.Id);

        Assert.IsType<PageResult>(result);
        Assert.Equal(secondReferral.Id, pageModel.Referral.Id);
        Assert.Equal("DETAIL-004-B", pageModel.Referral.Reference);

        // Viewing Details must not change or delete records.
        Assert.Equal(2, await context.Referrals.CountAsync());
    }

    // Reusable fictional referral for test setup.
    private static Referral CreateTestReferral(string reference)
    {
        return new Referral
        {
            Reference = reference,
            ServiceName = "Community Physiotherapy",
            Status = ReferralStatus.InProgress,
            Priority = ReferralPriority.Urgent,
            ReferralDate = new DateTime(2026, 10, 8),
            Summary = "Fictional referral for Details testing",
            AssignedTeam = "North Community Team"
        };
    }
}
