
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityHealthServiceTracker.Models;
using Xunit;

namespace CommunityHealthServiceTracker.Tests;

public class ReferralValidationTests
{
    [Fact]
    public void Referral_WithEmptyReference_ShouldFailValidation()
    {
        // Arrange
        var referral = new Referral
        {
            Reference = "",
            ServiceName = "Community Physiotherapy",
            Summary = "Fictional referral for testing"
        };

        var context = new ValidationContext(referral);
        var results = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(
            referral,
            context,
            results,
            validateAllProperties: true
        );

        // Assert
        Assert.False(isValid);

        Assert.Contains(results, result =>
            result.MemberNames.Contains(nameof(Referral.Reference)));
    }

    [Fact]
    public void Referral_WithReferenceLongerThan20Characters_ShouldFailValidation()
    {
        // Arrange
        var referral = new Referral
        {
            Reference = new string('A', 21),
            ServiceName = "Community Physiotherapy",
            Summary = "Fictional referral for testing"
        };

        var context = new ValidationContext(referral);
        var results = new List<ValidationResult>();

        // Act
        bool isValid = Validator.TryValidateObject(
            referral,
            context,
            results,
            validateAllProperties: true
        );

        // Assert
        Assert.False(isValid);

        Assert.Contains(results, result =>
            result.MemberNames.Contains(nameof(Referral.Reference)));
    }
    [Fact]
public void Referral_WithValidData_ShouldPassValidation()
{
    // Arrange
    var referral = new Referral
    {
        Reference = "REF-2026-002",
        ServiceName = "Community Physiotherapy",
        Summary = "Fictional referral for testing",
        Status = ReferralStatus.New,
        Priority = ReferralPriority.Routine,
        ReferralDate = new DateTime(2026, 10, 8)
    };

    var context = new ValidationContext(referral);
    var results = new List<ValidationResult>();

    // Act
    bool isValid = Validator.TryValidateObject(
        referral,
        context,
        results,
        validateAllProperties: true
    );

    // Assert
    Assert.True(isValid);
    Assert.Empty(results);
}
}
