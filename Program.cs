using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Kestrel para Railway
builder.WebHost.ConfigureKestrel(options =>
{
	var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
	options.ListenAnyIP(int.Parse(port));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000",
						  "http://localhost:5173", "http://127.0.0.1:5173",
						  "http://localhost:8080", "http://127.0.0.1:8080")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});

	// Para desarrollo: permitir cualquier origen si es necesario
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS - En desarrollo usar AllowAll para mayor flexibilidad
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Endpoint raíz opcional para pruebas en Railway
app.MapGet("/", () => "Backend .NET 8 en Railway funcionando 🚀");

app.Run();