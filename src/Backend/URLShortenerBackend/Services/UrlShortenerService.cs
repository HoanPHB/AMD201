using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using URLShortenerBackend.Data;
using URLShortenerBackend.Models;

namespace URLShortenerBackend.Services
{
    public class UrlShortenerService
    {
        private readonly UrlShortenerDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<UrlShortenerService> _logger;

        public UrlShortenerService(UrlShortenerDbContext dbContext, IMemoryCache memoryCache, ILogger<UrlShortenerService> logger)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
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
                // Cache the existing mapping
                CacheUrl(existing.ShortCode, existing.OriginalUrl, existing.ExpiresAt);
                return existing.ShortCode; // Return the existing short code
            }

            // Generate a unique short code
            var shortCode = GenerateShortCode();

            // Set expiration time to 1 day from now
            var createdAt = DateTime.UtcNow;
            var calculatedExpiresAt = expiresAt ?? createdAt.AddDays(1);

            // Save to the database
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = createdAt,
                ExpiresAt = calculatedExpiresAt
            };

            _dbContext.ShortUrls.Add(shortUrl);
            await _dbContext.SaveChangesAsync();

            // Cache the new mapping
            CacheUrl(shortCode, originalUrl, calculatedExpiresAt);

            _logger.LogInformation("Created new short URL with expiration date: {ExpiresAt}", calculatedExpiresAt);

            return shortCode;
        }

        // Method to retrieve the original URL based on the short code
        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            _logger.LogInformation("Retrieving original URL for short code: {ShortCode}", shortCode);

            // Check the cache first
            if (_memoryCache.TryGetValue(shortCode, out CachedUrl cachedUrl))
            {
                // Validate expiration
                if (cachedUrl.ExpiresAt.HasValue && cachedUrl.ExpiresAt.Value < DateTime.UtcNow)
                {
                    _logger.LogWarning("Cached short URL with code {ShortCode} has expired", shortCode);
                    _memoryCache.Remove(shortCode); // Remove expired cache entry
                    throw new Exception("The shortened URL has expired.");
                }

                _logger.LogInformation("Cache hit for short code: {ShortCode}", shortCode);
                return cachedUrl.OriginalUrl;
            }

            _logger.LogInformation("Cache miss for short code: {ShortCode}", shortCode);

            // If not in cache, query the database
            var shortUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (shortUrl == null)
            {
                _logger.LogWarning("Short URL with code {ShortCode} not found", shortCode);
                throw new Exception("Short URL not found");
            }

            // Check expiration
            if (shortUrl.ExpiresAt.HasValue && shortUrl.ExpiresAt.Value < DateTime.UtcNow)
            {
                _logger.LogWarning("Short URL with code {ShortCode} has expired", shortCode);
                throw new Exception("The shortened URL has expired.");
            }

            // Cache the retrieved URL
            CacheUrl(shortCode, shortUrl.OriginalUrl, shortUrl.ExpiresAt);

            _logger.LogInformation("Redirecting to original URL: {OriginalUrl}", shortUrl.OriginalUrl);
            return shortUrl.OriginalUrl;
        }

        // Utility to cache a URL
        private void CacheUrl(string shortCode, string originalUrl, DateTime? expiresAt)
        {
            var cacheOptions = new MemoryCacheEntryOptions();

            // Set absolute expiration if provided
            if (expiresAt.HasValue)
            {
                cacheOptions.SetAbsoluteExpiration(expiresAt.Value);
            }
            else
            {
                // Default expiration (1 day)
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromDays(1));
            }

            // Add to cache
            _memoryCache.Set(shortCode, new CachedUrl
            {
                OriginalUrl = originalUrl,
                ExpiresAt = expiresAt
            }, cacheOptions);

            _logger.LogInformation("Cached short code: {ShortCode} with expiration: {ExpiresAt}", shortCode, expiresAt);
        }

        // Utility to generate a unique short code
        private string GenerateShortCode()
        {
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(characters, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Nested class for caching
        private class CachedUrl
        {
            public string OriginalUrl { get; set; }
            public DateTime? ExpiresAt { get; set; }
        }
    }
}
