using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URLShortenerBackend.Data;
using URLShortenerBackend.Models;
using Microsoft.Extensions.Logging;
using URLShortenerBackend.Migrations;

namespace URLShortenerBackend.Services
{
    public class UrlShortenerService
    {
        private readonly UrlShortenerDbContext _dbContext;
        private readonly ILogger<UrlShortenerService> _logger;

        public UrlShortenerService(UrlShortenerDbContext dbContext, ILogger<UrlShortenerService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Method to create a short URL
        public async Task<string> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null)
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
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            _dbContext.ShortUrls.Add(shortUrl);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created new short URL with expiration date: {ExpiresAt}", expiresAt);

            return shortCode;
        }

        // Method to retrieve the original URL based on the short code
        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            _logger.LogInformation("Retrieving original URL for short code: {ShortCode}", shortCode);

            var shortUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (shortUrl == null)
            {
                _logger.LogWarning("Short URL with code {ShortCode} not found", shortCode);
                throw new Exception("Short URL not found");
            }

            if (shortUrl.ExpiresAt.HasValue && shortUrl.ExpiresAt.Value < DateTime.UtcNow)
            {
                _logger.LogWarning("Short URL with code {ShortCode} has expired", shortCode);
                throw new Exception("The shortened URL has expired.");
            }

            _logger.LogInformation("Redirecting to original URL: {OriginalUrl}", shortUrl.OriginalUrl);
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
