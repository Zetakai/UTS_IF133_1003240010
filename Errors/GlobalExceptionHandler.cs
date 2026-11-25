using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UTS_IF133_1003240010.Errors;

// Semua exception yang tidak ditangkap controller masuk ke sini.
// Stack trace-nya ditulis ke log, client hanya dapat ProblemDetails dan traceId.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        if (exception is DbUpdateException { InnerException: SqliteException { SqliteErrorCode: 19 } })
        {
            // SQLite error 19 = constraint gagal. Biasanya karena dua request menyimpan Code yang sama hampir bersamaan.
            _logger.LogWarning("Constraint database gagal di {Method} {Path}", context.Request.Method, context.Request.Path);
            problem = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Data bentrok dengan data yang sudah ada.",
                Detail = "Code produk sudah dipakai."
            };
        }
        else
        {
            _logger.LogError(exception, "Error tak terduga di {Method} {Path}", context.Request.Method, context.Request.Path);
            problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Terjadi kesalahan pada server.",
                Detail = "Silakan coba lagi. Jika masih terjadi, laporkan traceId ke admin."
            };
        }

        context.Response.StatusCode = problem.Status.Value;

        // Exception sengaja tidak dimasukkan ke context supaya pesannya tidak bocor ke response.
        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem
        });
    }
}
