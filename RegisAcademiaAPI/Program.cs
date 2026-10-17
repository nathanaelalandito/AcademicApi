using Microsoft.EntityFrameworkCore;
using AcademicApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Mendaftarkan Controller agar API bisa menerima request CRUD
builder.Services.AddControllers();

// (BARU) Tambahkan layanan Swagger/OpenAPI agar halaman dokumentasi bisa digenerate
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Mendaftarkan Database SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=academic.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.InitializeAsync(db);
}

// 3. Konfigurasi jalur HTTP dan aktifkan Middleware Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Inilah yang memunculkan tampilan halaman web Swagger!
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 4. Memetakan Controller ke endpoint API
app.MapControllers();

app.Run();