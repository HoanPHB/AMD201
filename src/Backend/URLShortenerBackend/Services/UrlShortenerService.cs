using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URLShortenerBackend.Data;
using URLShortenerBackend.Models;

namespace URLShortenerBackend.Services
{
    public class UrlShortenerService
    {
        private readonly UrlShortenerDbContext _dbContext;

        public UrlShortenerService(UrlShortenerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Method to create a short URL
        public async Task<string> CreateShortUrlAsync(string originalUrl)
        {
            // Validate input URL
            if (string.IsNullOrEmpty(originalUrl) || !Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
            {
                throw new ArgumentException("Invalid URL");
            }

            // Check if the URL already exists
            var existing = await _dbContext.ShortUrls.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);
            if (existing != null)
            {
                return existing.ShortCode; // Return the existing short code
            }

            // Generate a unique short code
            var shortCode = GenerateShortCode();

            // Save to the database
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ShortUrls.Add(shortUrl);
            await _dbContext.SaveChangesAsync();

            return shortCode;
        }

        // Method to retrieve the original URL based on the short code
        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            var shortUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (shortUrl == null)
            {
                throw new Exception("Short URL not found");
            }

            return shortUrl.OriginalUrl;
        }

        // Utility to generate a unique short code
        private string GenerateShortCode()
        {
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(characters, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
