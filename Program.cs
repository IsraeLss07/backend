using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de puertos - Railway usa PORT, desarrollo usa launchSettings
if (Environment.GetEnvironmentVariable("PORT") != null)
{
    // En Railway
    var port = Environment.GetEnvironmentVariable("PORT");
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}
// En desarrollo local, usar launchSettings.json (no configurar URLs manualmente)

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar CORS para Railway
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Backend API funcionando en Railway");

app.Run();