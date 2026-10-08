using CommunityHealthServiceTracker.Tests.Helpers;
using CommunityHealthServiceTracker.Data;
using CommunityHealthServiceTracker.Models;
using CommunityHealthServiceTracker.Pages.Referrals;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityHealthServiceTracker.Tests;

public class ReferralFilteringTests
{
    [Fact]
    public async Task OnGetAsync_WhenFilteringByStatus_ReturnsOnlyMatchingReferrals()
    {
        // Arrange: Create an isolated temporary test database.
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        // Add fictional referrals with different statuses.
        context.Referrals.AddRange(
            new Referral
            {
                Reference = "TEST-001",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional test referral",
                Status = ReferralStatus.New
            },
            new Referral
            {
                Reference = "TEST-002",
                ServiceName = "District Nursing",
                Summary = "Fictional test referral",
                Status = ReferralStatus.InProgress
            },
            new Referral
            {
                Reference = "TEST-003",
                ServiceName = "Occupational Therapy",
                Summary = "Fictional test referral",
                Status = ReferralStatus.Completed
            }
        );

        await context.SaveChangesAsync();

        // Create the same PageModel used by the Referrals page.
        var pageModel = new IndexModel(context)
        {
            Status = ReferralStatus.InProgress
        };

        // Act: Execute the page's filtering logic.
        await pageModel.OnGetAsync();

        // Assert: Only the In Progress referral should be returned.
        var referral = Assert.Single(pageModel.Referrals);

        Assert.Equal("TEST-002", referral.Reference);
        Assert.Equal(ReferralStatus.InProgress, referral.Status);
    }
    [Fact]
    public async Task OnGetAsync_WhenFilteringByPriority_ReturnsOnlyUrgentReferrals()
    {
        // Arrange: Create an isolated temporary test database.
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        // Add fictional referrals with different priorities.
        context.Referrals.AddRange(
            new Referral
            {
                Reference = "TEST-101",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional routine referral",
                Priority = ReferralPriority.Routine
            },
            new Referral
            {
                Reference = "TEST-102",
                ServiceName = "District Nursing",
                Summary = "Fictional urgent referral",
                Priority = ReferralPriority.Urgent
            },
            new Referral
            {
                Reference = "TEST-103",
                ServiceName = "Occupational Therapy",
                Summary = "Another fictional routine referral",
                Priority = ReferralPriority.Routine
            }
        );

        await context.SaveChangesAsync();

        var pageModel = new IndexModel(context)
        {
            Priority = ReferralPriority.Urgent
        };

        // Act
        await pageModel.OnGetAsync();

        // Assert
        var referral = Assert.Single(pageModel.Referrals);

        Assert.Equal("TEST-102", referral.Reference);
        Assert.Equal(ReferralPriority.Urgent, referral.Priority);
    }
    [Fact]
    public async Task OnGetAsync_WhenSearchingByServiceName_ReturnsMatchingReferral()
    {
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        context.Referrals.AddRange(
            new Referral
            {
                Reference = "TEST-201",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional test referral"
            },
            new Referral
            {
                Reference = "TEST-202",
                ServiceName = "District Nursing",
                Summary = "Fictional test referral"
            },
            new Referral
            {
                Reference = "TEST-203",
                ServiceName = "Occupational Therapy",
                Summary = "Fictional test referral"
            }
        );

        await context.SaveChangesAsync();

        var pageModel = new IndexModel(context)
        {
            SearchTerm = "Physiotherapy"
        };

        // Act: Run the actual Referrals page search.
        await pageModel.OnGetAsync();

        // Assert: Only the matching referral should appear.
        var referral = Assert.Single(pageModel.Referrals);

        Assert.Equal("TEST-201", referral.Reference);
        Assert.Equal("Community Physiotherapy", referral.ServiceName);
    }


[Fact]
    public async Task OnGetAsync_WithCombinedFilters_ReturnsOnlyMatchingReferral()
    {
      // Arrange: Create an isolated temporary test database.
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        // Create fictional referrals with different combinations.
        context.Referrals.AddRange(
            new Referral
            {
                Reference = "TEST-301",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional urgent referral in progress",
                Status = ReferralStatus.InProgress,
                Priority = ReferralPriority.Urgent
            },
            new Referral
            {
                Reference = "TEST-302",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional routine referral in progress",
                Status = ReferralStatus.InProgress,
                Priority = ReferralPriority.Routine
            },
            new Referral
            {
                Reference = "TEST-303",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional urgent completed referral",
                Status = ReferralStatus.Completed,
                Priority = ReferralPriority.Urgent
            },
            new Referral
            {
                Reference = "TEST-304",
                ServiceName = "District Nursing",
                Summary = "Fictional urgent referral in progress",
                Status = ReferralStatus.InProgress,
                Priority = ReferralPriority.Urgent
            }
        );

        await context.SaveChangesAsync();

        // Apply all three filters.
        var pageModel = new IndexModel(context)
        {
            SearchTerm = "Physiotherapy",
            Status = ReferralStatus.InProgress,
            Priority = ReferralPriority.Urgent
        };

        // Act: Execute the real Referrals page query.
        await pageModel.OnGetAsync();

        // Assert: Exactly one referral should match.
        var referral = Assert.Single(pageModel.Referrals);

        Assert.Equal("TEST-301", referral.Reference);
        Assert.Equal(ReferralStatus.InProgress, referral.Status);
        Assert.Equal(ReferralPriority.Urgent, referral.Priority);
    }

}
