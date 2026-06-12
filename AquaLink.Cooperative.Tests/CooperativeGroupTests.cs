using AquaLink.Cooperative.Domain.Entities;
using FluentAssertions;

namespace AquaLink.Cooperative.Tests;

public class CooperativeGroupTests
{
    private static CooperativeGroup CreateValidGroup()
        => CooperativeGroup.Create(
            "Epe Fish Farmers Cooperative",
            "Savings group for fish farmers",
            Guid.NewGuid(),
            "Adebayo Johnson",
            "08012345678");

    // ── Create ──────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidInputs_ShouldReturnActiveGroup()
    {
        var group = CreateValidGroup();

        group.Should().NotBeNull();
        group.Id.Should().NotBeEmpty();
        group.Name.Should().Be("Epe Fish Farmers Cooperative");
        group.Status.Should().Be(CooperativeStatus.Active);
        group.Members.Should().HaveCount(1);
        group.TreasurerMemberId.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldAddTreasurerAsFirstMember()
    {
        var group = CreateValidGroup();

        var treasurer = group.Members.First();
        treasurer.FullName.Should().Be("Adebayo Johnson");
        treasurer.PhoneNumber.Should().Be("08012345678");
        group.TreasurerMemberId.Should().Be(treasurer.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldThrow(string? name)
    {
        var act = () => CooperativeGroup.Create(
            name!, "description",
            Guid.NewGuid(), "John", "08012345678");

        act.Should().Throw<ArgumentException>();
    }

    // ── AddMember ────────────────────────────────────────────────────────

    [Fact]
    public void AddMember_WithNewUser_ShouldAddToMembersList()
    {
        var group = CreateValidGroup();
        var newUserId = Guid.NewGuid();

        var member = group.AddMember(newUserId, "Fatima Bello", "08098765432");

        group.Members.Should().HaveCount(2);
        member.FullName.Should().Be("Fatima Bello");
        member.Status.Should().Be(MemberStatus.Active);
    }

    [Fact]
    public void AddMember_WithDuplicateUser_ShouldThrowInvalidOperation()
    {
        var group = CreateValidGroup();
        var existingUserId = group.Members.First().UserId;

        var act = () => group.AddMember(
            existingUserId, "Duplicate User", "08011111111");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already a member*");
    }

    // ── RecordContribution ───────────────────────────────────────────────

    [Fact]
    public void RecordContribution_ByRegisteredMember_ShouldAddContribution()
    {
        var group = CreateValidGroup();
        var memberId = group.Members.First().Id;

        var contribution = group.RecordContribution(memberId, 5000m, "2026-06");

        group.Contributions.Should().HaveCount(1);
        contribution.AmountNaira.Should().Be(5000m);
        contribution.CycleMonth.Should().Be("2026-06");
        contribution.Status.Should().Be(ContributionStatus.Confirmed);
    }

    [Fact]
    public void RecordContribution_ByNonMember_ShouldThrowInvalidOperation()
    {
        var group = CreateValidGroup();
        var nonMemberId = Guid.NewGuid();

        var act = () => group.RecordContribution(
            nonMemberId, 5000m, "2026-06");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*registered members*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void RecordContribution_WithZeroOrNegativeAmount_ShouldThrow(
        decimal amount)
    {
        var group = CreateValidGroup();
        var memberId = group.Members.First().Id;

        var act = () => group.RecordContribution(memberId, amount, "2026-06");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*greater than zero*");
    }

    // ── TotalBalance ─────────────────────────────────────────────────────

    [Fact]
    public void TotalBalance_ShouldReflectConfirmedContributions()
    {
        var group = CreateValidGroup();
        var memberId = group.Members.First().Id;

        group.RecordContribution(memberId, 5000m, "2026-05");
        group.RecordContribution(memberId, 3000m, "2026-06");

        group.TotalBalance.Should().Be(8000m);
    }

    [Fact]
    public void TotalBalance_AfterApprovedWithdrawal_ShouldDeductAmount()
    {
        var group = CreateValidGroup();
        var treasurerId = group.Members.First().Id;

        group.RecordContribution(treasurerId, 10000m, "2026-06");

        var withdrawal = group.RequestWithdrawal(treasurerId, 3000m, "Equipment");
        group.ApproveWithdrawal(withdrawal.Id, treasurerId);

        group.TotalBalance.Should().Be(7000m);
    }

    // ── RequestWithdrawal ────────────────────────────────────────────────

    [Fact]
    public void RequestWithdrawal_ExceedingBalance_ShouldThrowInvalidOperation()
    {
        var group = CreateValidGroup();
        var memberId = group.Members.First().Id;

        group.RecordContribution(memberId, 5000m, "2026-06");

        var act = () => group.RequestWithdrawal(memberId, 10000m, "Too much");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient balance*");
    }

    [Fact]
    public void RequestWithdrawal_ByNonMember_ShouldThrow()
    {
        var group = CreateValidGroup();
        var memberId = group.Members.First().Id;
        group.RecordContribution(memberId, 5000m, "2026-06");

        var act = () => group.RequestWithdrawal(
            Guid.NewGuid(), 1000m, "Reason");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*registered members*");
    }

    // ── ApproveWithdrawal ────────────────────────────────────────────────

    [Fact]
    public void ApproveWithdrawal_ByTreasurer_ShouldSetStatusToApproved()
    {
        var group = CreateValidGroup();
        var treasurerId = group.Members.First().Id;

        group.RecordContribution(treasurerId, 10000m, "2026-06");
        var withdrawal = group.RequestWithdrawal(
            treasurerId, 3000m, "Equipment purchase");

        group.ApproveWithdrawal(withdrawal.Id, treasurerId);

        withdrawal.Status.Should().Be(WithdrawalStatus.Approved);
        withdrawal.ApprovedByMemberId.Should().Be(treasurerId);
        withdrawal.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public void ApproveWithdrawal_ByNonTreasurer_ShouldThrowInvalidOperation()
    {
        var group = CreateValidGroup();
        var treasurerId = group.Members.First().Id;
        var otherMember = group.AddMember(
            Guid.NewGuid(), "Other Member", "08011111111");

        group.RecordContribution(treasurerId, 10000m, "2026-06");
        var withdrawal = group.RequestWithdrawal(
            treasurerId, 3000m, "Equipment");

        var act = () => group.ApproveWithdrawal(withdrawal.Id, otherMember.Id);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*treasurer*");
    }
}