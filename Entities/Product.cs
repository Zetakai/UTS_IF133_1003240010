namespace UTS_IF133_1003240010.Entities;

public class Product
{
    public int Id { get; set; }

    // Kode produk, misalnya PRD-001. Harus unik.
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    // false = produk sudah tidak dijual.
    public bool IsActive { get; set; } = true;
}
