using Microsoft.EntityFrameworkCore;
using UTS_IF133_1003240010.Entities;

namespace UTS_IF133_1003240010.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Code).IsRequired().HasMaxLength(20);
            // Controller sudah cek Code duplikat, tapi dua request bersamaan bisa lolos dua-duanya. Unique index ini yang menahan.
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            // SQLite tidak punya tipe decimal, jadi harga disimpan sebagai TEXT biar tidak kena pembulatan.
            entity.Property(p => p.Price).HasColumnType("TEXT");
        });
    }
}
