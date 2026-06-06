using AquaLink.Prices.Application.Prices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaLink.Farmer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricesController : ControllerBase
{
    private readonly ISender _sender;

    public PricesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "FieldAgent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitPrice(
        [FromBody] SubmitPriceCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = await _sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(SubmitPrice), new { id }, new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("current")]
    [ProducesResponseType(typeof(List<CurrentPriceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentPrices(
        [FromQuery] string? market,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCurrentPricesQuery(market), cancellationToken);
        return Ok(result);
    }
}