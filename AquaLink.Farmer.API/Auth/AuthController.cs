using Microsoft.AspNetCore.Mvc;

namespace AquaLink.Farmer.API.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetToken([FromBody] TokenRequest request)
    {
        // Development only — in production this validates
        // against the Identity/farmer database
        var token = _tokenService.GenerateToken(
            request.UserId,
            request.Role);

        return Ok(new { token });
    }
}

public record TokenRequest(Guid UserId, string Role);