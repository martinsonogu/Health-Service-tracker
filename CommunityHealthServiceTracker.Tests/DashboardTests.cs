
using CommunityHealthServiceTracker.Models;
using CommunityHealthServiceTracker.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

// Alias avoids confusion with the Referrals IndexModel.
using DashboardModel = CommunityHealthServiceTracker.Pages.IndexModel;

namespace CommunityHealthServiceTracker.Tests;

public class DashboardTests
{
    // TEST 1: An empty database produces zero counts.
    [Fact]
    public async Task OnGetAsync_WithNoReferrals_ShowsZeroCounts()
    {
        await using var database = await TestDatabase.CreateAsync();

        var pageModel = new DashboardModel(database.Context);

        await pageModel.OnGetAsync();

        Assert.Equal(0, pageModel.TotalReferrals);
        Assert.Equal(0, pageModel.NewReferrals);
        Assert.Equal(0, pageModel.InProgressReferrals);
        Assert.Equal(0, pageModel.CompletedReferrals);
        Assert.Equal(0, pageModel.UrgentReferrals);
        Assert.Empty(pageModel.RecentReferrals);
    }

    // TEST 2: Dashboard calculates all status counts correctly.
    [Fact]
    public async Task OnGetAsync_WithReferrals_CalculatesStatusCounts()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        context.Referrals.AddRange(
            CreateTestReferral("DASH-001", ReferralStatus.New),
            CreateTestReferral("DASH-002", ReferralStatus.New),
            CreateTestReferral("DASH-003", ReferralStatus.InProgress),
            CreateTestReferral("DASH-004", ReferralStatus.Completed)
        );

        await context.SaveChangesAsync();

        var pageModel = new DashboardModel(context);

        await pageModel.OnGetAsync();

        Assert.Equal(4, pageModel.TotalReferrals);
        Assert.Equal(2, pageModel.NewReferrals);
        Assert.Equal(1, pageModel.InProgressReferrals);
        Assert.Equal(1, pageModel.CompletedReferrals);
    }

    // TEST 3: Urgent referrals are counted across all statuses.
    [Fact]
    public async Task OnGetAsync_CountsUrgentReferrals()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        context.Referrals.AddRange(
            CreateTestReferral(
                "DASH-005",
                ReferralStatus.New,
                ReferralPriority.Urgent),

            CreateTestReferral(
                "DASH-006",
                ReferralStatus.InProgress,
                ReferralPriority.Urgent),

            CreateTestReferral(
                "DASH-007",
                ReferralStatus.Completed,
                ReferralPriority.Routine)
        );

        await context.SaveChangesAsync();

        var pageModel = new DashboardModel(context);

        await pageModel.OnGetAsync();

        Assert.Equal(3, pageModel.TotalReferrals);
        Assert.Equal(2, pageModel.UrgentReferrals);
    }

    // TEST 4: Recent referrals are ordered newest first.
    [Fact]
    public async Task OnGetAsync_OrdersRecentReferralsByDateDescending()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        context.Referrals.AddRange(
            CreateTestReferral(
                "DASH-OLD",
                date: new DateTime(2026, 10, 1)),

            CreateTestReferral(
                "DASH-NEW",
                date: new DateTime(2026, 10, 8)),

            CreateTestReferral(
                "DASH-MIDDLE",
                date: new DateTime(2026, 10, 5))
        );

        await context.SaveChangesAsync();

        var pageModel = new DashboardModel(context);

        await pageModel.OnGetAsync();

        var references = pageModel.RecentReferrals
            .Select(r => r.Reference)
            .ToArray();

        Assert.Equal(
            new[] { "DASH-NEW", "DASH-MIDDLE", "DASH-OLD" },
            references);
    }

    // TEST 5: Dashboard displays no more than five recent referrals.
    [Fact]
    public async Task OnGetAsync_ReturnsOnlyFiveRecentReferrals()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        for (int day = 1; day <= 8; day++)
        {
            context.Referrals.Add(
                CreateTestReferral(
                    $"DASH-{day:000}",
                    date: new DateTime(2026, 10, day)));
        }

        await context.SaveChangesAsync();

        var pageModel = new DashboardModel(context);

        await pageModel.OnGetAsync();

        Assert.Equal(8, pageModel.TotalReferrals);
        Assert.Equal(5, pageModel.RecentReferrals.Count);

        Assert.Equal(
            new[]
            {
                "DASH-008",
                "DASH-007",
                "DASH-006",
                "DASH-005",
                "DASH-004"
            },
            pageModel.RecentReferrals
                .Select(r => r.Reference)
                .ToArray());
    }

    // TEST 6: Referrals with the same date use ID as a tiebreaker.
    [Fact]
    public async Task OnGetAsync_WithSameDate_OrdersByIdDescending()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var date = new DateTime(2026, 10, 8);

        var first = CreateTestReferral("DASH-FIRST", date: date);
        var second = CreateTestReferral("DASH-SECOND", date: date);

        context.Referrals.AddRange(first, second);
        await context.SaveChangesAsync();

        var pageModel = new DashboardModel(context);

        await pageModel.OnGetAsync();

        Assert.Equal(
            new[] { "DASH-SECOND", "DASH-FIRST" },
            pageModel.RecentReferrals
                .Select(r => r.Reference)
                .ToArray());
    }

    // Reusable fictional referral for test setup.
    private static Referral CreateTestReferral(
        string reference,
        ReferralStatus status = ReferralStatus.New,
        ReferralPriority priority = ReferralPriority.Routine,
        DateTime? date = null)
    {
        return new Referral
        {
            Reference = reference,
            ServiceName = "Community Physiotherapy",
            Status = status,
            Priority = priority,
            ReferralDate = date ?? new DateTime(2026, 10, 8),
            Summary = "Fictional referral for dashboard testing",
            AssignedTeam = "North Community Team"
        };
    }
}
