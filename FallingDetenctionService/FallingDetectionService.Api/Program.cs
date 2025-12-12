
using FallingDetectionService.Infrastructure.Interfaces;
using FallingDetectionService.Infrastructure.Persistence;
using FallingDetectionService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext для PostgreSQL
builder.Services.AddDbContext<SafetyDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// Реєструємо сервіси (з Infrastructure)
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<IZoneService, ZoneService>();


builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// swagger для тесту
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");


app.MapControllers();
app.Run();