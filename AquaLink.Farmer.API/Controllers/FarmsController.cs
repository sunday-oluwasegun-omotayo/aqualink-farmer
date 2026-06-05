using AquaLink.Farmer.Application.FarmCycles;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace AquaLink.Farmer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FarmsController : ControllerBase
{
    private readonly ISender _sender;

    public FarmsController(ISender sender)
    {
        _sender = sender;
    }


    [HttpPost]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFarmCycle(
        [FromBody] CreateFarmCycleCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetFarmCycle), new { id }, new { id });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                errors = ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FarmCycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFarmCycle(
    Guid id,
    CancellationToken cancellationToken)
    {
        try
        {
            var result = await _sender.Send(new GetFarmCycleQuery(id), cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/harvest")]
    [Authorize(Roles = "Farmer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordHarvest(
    Guid id,
    [FromBody] RecordHarvestRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            await _sender.Send(
                new RecordHarvestCommand(
                    id,
                    request.HarvestedWeightKg,
                    request.SalePricePerKg,
                    request.HarvestedAt),
                cancellationToken);

            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                errors = ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                })
            });
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

    [HttpGet]
    [ProducesResponseType(typeof(List<FarmCycleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFarmCycles(
    [FromQuery] Guid farmerId,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetFarmCyclesQuery(farmerId),
            cancellationToken);

        return Ok(result);
    }

}
public record RecordHarvestRequest(
       decimal HarvestedWeightKg,
       decimal SalePricePerKg,
       DateOnly HarvestedAt);