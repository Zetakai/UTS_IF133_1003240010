using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using UTS_IF133_1003240010.Data;
using UTS_IF133_1003240010.Errors;

var builder = WebApplication.CreateBuilder(args);

// Log satu baris per kejadian dengan jam di depannya, biar gampang dibaca di terminal.
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

// Response 400 bawaan saya ganti supaya judulnya berbahasa Indonesia dan validasi yang gagal ikut tercatat di log.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var http = context.HttpContext;
            var logger = http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Validation");

            // Yang dicatat cuma nama field-nya. Isi datanya tidak, karena bisa saja berisi data sensitif.
            var fields = context.ModelState.Where(x => x.Value?.Errors.Count > 0).Select(x => x.Key);
            logger.LogWarning("Validasi gagal di {Method} {Path}, field: {Fields}",
                http.Request.Method, http.Request.Path, string.Join(", ", fields));

            var factory = http.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problem = factory.CreateValidationProblemDetails(http, context.ModelState,
                StatusCodes.Status400BadRequest, "Data yang dikirim tidak valid.");

            return new BadRequestObjectResult(problem) { ContentTypes = { "application/problem+json" } };
        };
    });

// Connection string ada di appsettings.json (DefaultConnection).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// instance dan traceId ditambahkan ke semua ProblemDetails, jadi error yang dilihat client bisa dicari di log.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Instance ??= ctx.HttpContext.Request.Path;
        ctx.ProblemDetails.Extensions["traceId"] = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "UTS IF133 - Inventory Product API",
        Version = "v1",
        Description = "Muhammad Farid Zaki - 1003240010"
    });
});

var app = builder.Build();

// Harus paling awal supaya exception dari middleware sesudahnya juga tertangkap.
app.UseExceptionHandler();
// Tanpa ini, route yang tidak ada dijawab 404 kosong tanpa body.
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Product API v1");
        // Swagger UI dibuka langsung di root: http://localhost:5086/
        options.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
