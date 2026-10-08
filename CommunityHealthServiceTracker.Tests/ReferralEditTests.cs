
using CommunityHealthServiceTracker.Models;
using CommunityHealthServiceTracker.Pages.Referrals;
using CommunityHealthServiceTracker.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommunityHealthServiceTracker.Tests;

public class ReferralEditTests
{
    // TEST 1: GET request loads the correct referral.
    [Fact]
    public async Task OnGetAsync_WithValidId_LoadsReferral()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var referral = CreateTestReferral("EDIT-001");
        context.Referrals.Add(referral);
        await context.SaveChangesAsync();

        var pageModel = new EditModel(context);

        // Act
        var result = await pageModel.OnGetAsync(referral.Id);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal(referral.Id, pageModel.Input.Id);
        Assert.Equal("EDIT-001", pageModel.Input.Reference);
        Assert.Equal(referral.ServiceName, pageModel.Input.ServiceName);
        Assert.Equal(referral.Status, pageModel.Input.Status);
        Assert.Equal(referral.Priority, pageModel.Input.Priority);
        Assert.Equal(referral.ReferralDate, pageModel.Input.ReferralDate);
        Assert.Equal(referral.Summary, pageModel.Input.Summary);
        Assert.Equal(referral.AssignedTeam, pageModel.Input.AssignedTeam);
    }

    // TEST 2: GET request with no ID returns NotFound.
    [Fact]
    public async Task OnGetAsync_WithNullId_ReturnsNotFound()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();
        var pageModel = new EditModel(database.Context);

        // Act
        var result = await pageModel.OnGetAsync(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // TEST 3: GET request for a nonexistent referral returns NotFound.
    [Fact]
    public async Task OnGetAsync_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();
        var pageModel = new EditModel(database.Context);

        // Act
        var result = await pageModel.OnGetAsync(99999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // TEST 4: Valid POST updates all editable fields.
    [Fact]
    public async Task OnPostAsync_WithValidInput_UpdatesReferralAndRedirects()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var referral = CreateTestReferral("EDIT-004");
        context.Referrals.Add(referral);
        await context.SaveChangesAsync();

        var updatedDate = new DateTime(2026, 10, 9);

        var pageModel = new EditModel(context)
        {
            Input = new EditReferralInput
            {
                Id = referral.Id,
                Reference = "EDIT-UPDATED-004",
                ServiceName = "District Nursing",
                Status = ReferralStatus.Completed,
                Priority = ReferralPriority.Urgent,
                ReferralDate = updatedDate,
                Summary = "Updated fictional referral summary",
                AssignedTeam = "South Community Team"
            }
        };

        // Act
        var result = await pageModel.OnPostAsync();

        // Assert: Redirect to the referrals listing.
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirect.PageName);

        // Clear EF Core tracking to read the saved database values.
        context.ChangeTracker.Clear();

        var savedReferral = await context.Referrals
            .AsNoTracking()
            .SingleAsync();

        Assert.Equal(referral.Id, savedReferral.Id);
        Assert.Equal("EDIT-UPDATED-004", savedReferral.Reference);
        Assert.Equal("District Nursing", savedReferral.ServiceName);
        Assert.Equal(ReferralStatus.Completed, savedReferral.Status);
        Assert.Equal(ReferralPriority.Urgent, savedReferral.Priority);
        Assert.Equal(updatedDate, savedReferral.ReferralDate);
        Assert.Equal(
            "Updated fictional referral summary",
            savedReferral.Summary);
        Assert.Equal("South Community Team", savedReferral.AssignedTeam);
    }

    // TEST 5: Invalid model state must not save changes.
    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_DoesNotUpdateReferral()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var referral = CreateTestReferral("EDIT-005");
        context.Referrals.Add(referral);
        await context.SaveChangesAsync();

        var originalSummary = referral.Summary;

        var pageModel = new EditModel(context)
        {
            Input = new EditReferralInput
            {
                Id = referral.Id,
                Reference = "",
                ServiceName = "Community Physiotherapy",
                Status = ReferralStatus.New,
                Priority = ReferralPriority.Routine,
                ReferralDate = referral.ReferralDate,
                Summary = "This change must not be saved",
                AssignedTeam = "North Community Team"
            }
        };

        // Simulate a validation error.
        pageModel.ModelState.AddModelError(
            "Input.Reference",
            "The Reference field is required.");

        // Act
        var result = await pageModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);

        context.ChangeTracker.Clear();

        var savedReferral = await context.Referrals
            .AsNoTracking()
            .SingleAsync();

        Assert.Equal("EDIT-005", savedReferral.Reference);
        Assert.Equal(originalSummary, savedReferral.Summary);
    }

    // TEST 6: POST for a nonexistent referral returns NotFound.
    [Fact]
    public async Task OnPostAsync_WithNonexistentId_ReturnsNotFound()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();

        var pageModel = new EditModel(database.Context)
        {
            Input = new EditReferralInput
            {
                Id = 99999,
                Reference = "EDIT-999",
                ServiceName = "Community Physiotherapy",
                Summary = "Fictional referral",
                Status = ReferralStatus.New,
                Priority = ReferralPriority.Routine,
                ReferralDate = new DateTime(2026, 10, 8)
            }
        };

        // Act
        var result = await pageModel.OnPostAsync();

        // Assert
        Assert.IsType<NotFoundResult>(result);
        Assert.Empty(await database.Context.Referrals.ToListAsync());
    }

    // TEST 7: Editing one referral must not change another.
    [Fact]
    public async Task OnPostAsync_UpdatesOnlyTheSelectedReferral()
    {
        // Arrange
        await using var database = await TestDatabase.CreateAsync();
        var context = database.Context;

        var firstReferral = CreateTestReferral("EDIT-007-A");
        var secondReferral = CreateTestReferral("EDIT-007-B");

        context.Referrals.AddRange(firstReferral, secondReferral);
        await context.SaveChangesAsync();

        var originalSecondSummary = secondReferral.Summary;

        var pageModel = new EditModel(context)
        {
            Input = new EditReferralInput
            {
                Id = firstReferral.Id,
                Reference = firstReferral.Reference,
                ServiceName = firstReferral.ServiceName,
                Status = ReferralStatus.InProgress,
                Priority = ReferralPriority.Urgent,
                ReferralDate = firstReferral.ReferralDate,
                Summary = "Only the first referral was updated",
                AssignedTeam = firstReferral.AssignedTeam
            }
        };

        // Act
        var result = await pageModel.OnPostAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);

        context.ChangeTracker.Clear();

        var savedFirst = await context.Referrals
            .AsNoTracking()
            .SingleAsync(r => r.Id == firstReferral.Id);

        var savedSecond = await context.Referrals
            .AsNoTracking()
            .SingleAsync(r => r.Id == secondReferral.Id);

        Assert.Equal(
            "Only the first referral was updated",
            savedFirst.Summary);

        Assert.Equal(ReferralStatus.InProgress, savedFirst.Status);
        Assert.Equal(ReferralPriority.Urgent, savedFirst.Priority);

        Assert.Equal("EDIT-007-B", savedSecond.Reference);
        Assert.Equal(originalSecondSummary, savedSecond.Summary);
        Assert.Equal(ReferralStatus.New, savedSecond.Status);
        Assert.Equal(ReferralPriority.Routine, savedSecond.Priority);
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
            Summary = "Original fictional referral summary",
            AssignedTeam = "North Community Team"
        };
    }
}
