# UTS IF133 - Inventory Product API

Muhammad Farid Zaki (1003240010), Pemrograman .NET (IF133).

REST API sederhana untuk data produk inventori toko. Dibuat dengan ASP.NET Core Web API, Entity Framework Core, dan SQLite.

## Cara menjalankan

```bash
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

Setelah jalan, Swagger UI terbuka di http://localhost:5086/

## Endpoint

| Method | Endpoint | Berhasil | Gagal |
|---|---|---|---|
| GET | `/api/products` | 200 | - |
| GET | `/api/products/{id}` | 200 | 404 |
| POST | `/api/products` | 201 + `Location` | 400 (validasi), 409 (Code duplikat) |
| PUT | `/api/products/{id}` | 204 | 400, 404, 409 |
| DELETE | `/api/products/{id}` | 204 | 404 |
| GET | `/api/diagnostics/error` | - | 500 (simulasi, hanya Development) |

Semua error dikirim dalam format ProblemDetails dengan `traceId`. Detail exception tidak pernah dikirim ke client; detailnya hanya ada di log server.

## Isi folder

- `Controllers/`, `Data/`, `Dtos/`, `Entities/`, `Errors/`, `Migrations/`: source code
- `inventory.db`: database SQLite hasil migration
- `docs/screenshots/`: screenshot migration dan pengujian di Swagger
- `docs/evidence/`: output terminal waktu migration dan log server

Jawaban Soal 1 dan 3, penjelasan implementasi, dan bukti pengujian ada di laporan PDF yang dikumpulkan terpisah.
