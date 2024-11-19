using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using URLShortenerBackend.Data;
using URLShortenerBackend.Services;




var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
//    options.UseSqlite("Data Source=UrlShortener.db")); // SQLite database file
builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddEntityFrameworkMySQL()
    .AddDbContext<DbContext>(options =>
    {
        options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // Size limit in units (can be adjusted)
});


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
