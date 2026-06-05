using AquaLink.Farmer.Application.FarmCycles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AquaLink.Farmer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FarmsController : ControllerBase
{
    private readonly ISender _sender;

    public FarmsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFarmCycle(
        [FromBody] CreateFarmCycleCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateFarmCycle), new { id }, new { id });
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
public record RecordHarvestRequest(
       decimal HarvestedWeightKg,
       decimal SalePricePerKg,
       DateOnly HarvestedAt);