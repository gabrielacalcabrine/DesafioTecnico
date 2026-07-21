using Microsoft.AspNetCore.Mvc;
using Trading.Application.Services;
using Trading.Application.Services.Interfaces;

namespace Trading.Api.Controllers;

[ApiController]
[Route("admin")]
public sealed class AdminController(IAdminService admin, IConfiguration configuration) : ControllerBase
{
    [HttpPost("reset")]
    public async Task<IActionResult> Reset(CancellationToken cancellationToken)
    {
        if (!configuration.GetValue("ENABLE_TEST_ENDPOINTS", false)) return NotFound();
        await admin.ResetAsync(cancellationToken);
        return Ok(new { status = "reset" });
    }
}
