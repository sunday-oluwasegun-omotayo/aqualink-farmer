using AquaLink.Cooperative.Application.Cooperatives;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaLink.Farmer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CooperativesController : ControllerBase
{
    private readonly ISender _sender;

    public CooperativesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCooperative(
        [FromBody] CreateCooperativeGroupCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(CreateCooperative),
                new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMember(
        Guid id,
        [FromBody] AddMemberRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var memberId = await _sender.Send(
                new AddMemberCommand(
                    id,
                    request.UserId,
                    request.FullName,
                    request.PhoneNumber),
                cancellationToken);

            return CreatedAtAction(nameof(AddMember),
                new { id, memberId }, new { memberId });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/contributions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordContribution(
        Guid id,
        [FromBody] RecordContributionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contributionId = await _sender.Send(
                new RecordContributionCommand(
                    id,
                    request.MemberId,
                    request.AmountNaira,
                    request.CycleMonth),
                cancellationToken);

            return CreatedAtAction(nameof(RecordContribution),
                new { id, contributionId }, new { contributionId });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CooperativeGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCooperative(
    Guid id,
    CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(
                new GetCooperativeGroupQuery(id), cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/withdrawals")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestWithdrawal(
        Guid id,
        [FromBody] RequestWithdrawalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var withdrawalId = await _sender.Send(
                new RequestWithdrawalCommand(
                    id,
                    request.RequestedByMemberId,
                    request.AmountNaira,
                    request.Reason),
                cancellationToken);

            return CreatedAtAction(nameof(RequestWithdrawal),
                new { id, withdrawalId }, new { withdrawalId });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/withdrawals/{withdrawalId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveWithdrawal(
        Guid id,
        Guid withdrawalId,
        [FromBody] ApproveWithdrawalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new ApproveWithdrawalCommand(id, withdrawalId,
                    request.ApprovedByMemberId),
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record AddMemberRequest(
    Guid UserId,
    string FullName,
    string PhoneNumber);

public record RecordContributionRequest(
    Guid MemberId,
    decimal AmountNaira,
    string CycleMonth);

public record RequestWithdrawalRequest(
    Guid RequestedByMemberId,
    decimal AmountNaira,
    string Reason);

public record ApproveWithdrawalRequest(
    Guid ApprovedByMemberId);