using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTS_IF133_1003240010.Data;
using UTS_IF133_1003240010.Dtos;
using UTS_IF133_1003240010.Entities;

namespace UTS_IF133_1003240010.Controllers;

// [ApiController] sudah mengecek DTO duluan. Kalau tidak valid, langsung 400 dan action tidak dijalankan.
[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext db, ILogger<ProductsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _db.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new ProductDto(p.Id, p.Code, p.Name, p.Price, p.Stock, p.IsActive))
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return ProductNotFound(id);
        }

        return Ok(ToDto(product));
    }

    // CreatedAtAction mengisi header Location dengan alamat produk baru (GET /api/products/{id}).
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var code = dto.Code!.Trim();

        if (await _db.Products.AnyAsync(p => p.Code == code))
        {
            return CodeConflict(code);
        }

        var product = new Product
        {
            Code = code,
            Name = dto.Name!.Trim(),
            Price = dto.Price!.Value,
            Stock = dto.Stock!.Value,
            IsActive = dto.IsActive
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Produk {Id} ({Code}) ditambahkan", product.Id, product.Code);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToDto(product));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return ProductNotFound(id);
        }

        var code = dto.Code!.Trim();

        // p.Id != id: kalau Code-nya tidak diganti, jangan dianggap bentrok dengan dirinya sendiri.
        if (await _db.Products.AnyAsync(p => p.Code == code && p.Id != id))
        {
            return CodeConflict(code);
        }

        product.Code = code;
        product.Name = dto.Name!.Trim();
        product.Price = dto.Price!.Value;
        product.Stock = dto.Stock!.Value;
        product.IsActive = dto.IsActive!.Value;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Produk {Id} diubah", id);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return ProductNotFound(id);
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Produk {Id} ({Code}) dihapus", id, product.Code);
        return NoContent();
    }

    private static ProductDto ToDto(Product p) => new(p.Id, p.Code, p.Name, p.Price, p.Stock, p.IsActive);

    // Pakai Problem() supaya 404 dan 409 formatnya sama dengan error lain (ProblemDetails).
    private ObjectResult ProductNotFound(int id)
    {
        _logger.LogWarning("Produk {Id} tidak ditemukan", id);
        return Problem(statusCode: StatusCodes.Status404NotFound,
            title: "Produk tidak ditemukan.",
            detail: $"Produk dengan id {id} tidak ada di database.");
    }

    private ObjectResult CodeConflict(string code)
    {
        _logger.LogWarning("Code {Code} sudah dipakai", code);
        return Problem(statusCode: StatusCodes.Status409Conflict,
            title: "Code produk sudah dipakai.",
            detail: $"Produk dengan code '{code}' sudah ada.");
    }
}
