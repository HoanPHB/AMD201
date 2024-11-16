using Microsoft.EntityFrameworkCore;
using URLShortenerBackend.Models;

namespace URLShortenerBackend.Data
{
    public class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : base(options)
        {
        }

        // Define the table for Shortened URLs
        public DbSet<ShortUrl> ShortUrls { get; set; }
    }
}
