using Microsoft.AspNetCore.Mvc;

namespace UTS_IF133_1003240010.Controllers;

// Endpoint ini cuma untuk mengetes error 500. Di luar Development jawabannya 404.
[ApiController]
[Route("api/diagnostics")]
[Produces("application/json")]
public class DiagnosticsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public DiagnosticsController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet("error")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult ThrowError()
    {
        if (!_env.IsDevelopment())
        {
            return NotFound();
        }

        // Pesan ini harusnya cuma muncul di log server, tidak di response.
        throw new InvalidOperationException("Simulasi error internal: koneksi database putus.");
    }
}
