using Microsoft.AspNetCore.Mvc;
using URLShortenerBackend.Services;
using Microsoft.Extensions.Logging;

namespace URLShortenerBackend.Controllers
{
    [ApiController]
    [Route("url")]
    public class UrlController : ControllerBase
    {
        public class ShortenUrlRequest
        {
            public string OriginalUrl { get; set; }
            public DateTime? ExpiresAt { get; set; } // Optional expiration date
        }

        private readonly UrlShortenerService _urlShortenerService;
        private readonly ILogger<UrlController> _logger;

        public UrlController(UrlShortenerService urlShortenerService, ILogger<UrlController> logger)
        {
            _urlShortenerService = urlShortenerService;
            _logger = logger;
        }

        // POST /url/shorten
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
            {
                _logger.LogWarning("Original URL cannot be null or empty.");
                return BadRequest(new { error = "Original URL cannot be empty." });
            }

            try
            {
                var shortCode = await _urlShortenerService.CreateShortUrlAsync(request.OriginalUrl, request.ExpiresAt);
                var shortUrl = $"{Request.Scheme}://{Request.Host}/url/{shortCode}";

                _logger.LogInformation($"Short URL created: {shortUrl}");
                return Ok(new { shortUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a short URL.");
                return StatusCode(500, new { error = "An error occurred while creating the short URL." });
            }
        }

        // GET /url/{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> GetOriginalUrl(string code, bool returnUrl = false)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("Short code cannot be null or empty.");
                return BadRequest(new { error = "Short code cannot be empty." });
            }

            try
            {
                var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(code);
                if (originalUrl == null)
                {
                    _logger.LogWarning($"No URL found for short code: {code}");
                    return NotFound(new { error = "No URL found for the given short code." });
                }

                _logger.LogInformation($"Processing short code {code}: {originalUrl}");

                if (returnUrl)
                {

                    return Ok(new { originalUrl });
                }
                else
                {
                    return Redirect(originalUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resolving short code: {code}");
                if (ex.Message == "The shortened URL has expired.")
                {
                    return BadRequest(new { error = "This URL has expired. Please create a new one." });
                }
                return StatusCode(500, new { error = "An error occurred while processing the request." });
            }
        }

        // GET /url/analytics/{code}
        [HttpGet("analytics/{code}")]
        public async Task<IActionResult> GetAnalytics(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("Short code cannot be null or empty for analytics.");
                return BadRequest(new { error = "Short code cannot be empty." });
            }

            try
            {
                var analytics = await _urlShortenerService.GetAnalyticsAsync(code);

                if (analytics == null)
                {
                    _logger.LogWarning($"No analytics found for short code: {code}");
                    return NotFound(new { error = "No analytics found for the given short code." });
                }

                _logger.LogInformation($"Retrieved analytics for short code: {code}");
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analytics.");
                return StatusCode(500, new { error = "An error occurred while retrieving analytics." });
            }
        }
    }
}
