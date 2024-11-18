using Microsoft.EntityFrameworkCore;
using URLShortenerBackend.Data;
using URLShortenerBackend.Services;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
    options.UseSqlite("Data Source=UrlShortener.db")); // SQLite database file
builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Build the application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map the controllers to routes
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Start the application
app.Run();
