using System.ComponentModel.DataAnnotations;

namespace UTS_IF133_1003240010.Dtos;

// Body untuk POST /api/products. Tidak ada Id karena Id dibuat oleh database.
public class CreateProductDto
{
    [Required(ErrorMessage = "Code wajib diisi.")]
    [StringLength(20, ErrorMessage = "Code maksimal 20 karakter.")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Name wajib diisi.")]
    [StringLength(100, ErrorMessage = "Name maksimal 100 karakter.")]
    public string? Name { get; set; }

    // Sengaja decimal? (nullable). Kalau decimal biasa, Price yang lupa dikirim jadi 0 dan lolos [Required].
    [Required(ErrorMessage = "Price wajib diisi.")]
    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Price harus lebih besar dari 0.")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Stock wajib diisi.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock tidak boleh negatif.")]
    public int? Stock { get; set; }

    // Kalau tidak dikirim, produk dianggap aktif.
    public bool IsActive { get; set; } = true;
}

// Body untuk PUT /api/products/{id}. PUT mengganti seluruh data, jadi IsActive juga wajib dikirim.
public class UpdateProductDto
{
    [Required(ErrorMessage = "Code wajib diisi.")]
    [StringLength(20, ErrorMessage = "Code maksimal 20 karakter.")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Name wajib diisi.")]
    [StringLength(100, ErrorMessage = "Name maksimal 100 karakter.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Price wajib diisi.")]
    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Price harus lebih besar dari 0.")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Stock wajib diisi.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock tidak boleh negatif.")]
    public int? Stock { get; set; }

    [Required(ErrorMessage = "IsActive wajib diisi.")]
    public bool? IsActive { get; set; }
}

public record ProductDto(int Id, string Code, string Name, decimal Price, int Stock, bool IsActive);
