using Microsoft.AspNetCore.Mvc;
using URLShortenerBackend.Services;

namespace URLShortenerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        public class ShortenUrlRequest
        {
            public required string OriginalUrl { get; set; }
            public DateTime? ExpiresAt { get; set; } // Optional expiration date
        }

        private readonly UrlShortenerService _urlShortenerService;

        public UrlController(UrlShortenerService urlShortenerService)
        {
            _urlShortenerService = urlShortenerService;
        }

        // POST /shorten
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.OriginalUrl))
                {
                    return BadRequest(new { error = "Original URL cannot be empty." });
                }

                // Pass both the original URL and the optional expiration date
                var shortCode = await _urlShortenerService.CreateShortUrlAsync(request.OriginalUrl, request.ExpiresAt);
                var shortUrl = $"{Request.Scheme}://{Request.Host}/url/{shortCode}";

                return Ok(shortUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET /{code}
        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToOriginalUrl(string code)
        {
            try
            {
                var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(code);
                return Redirect(originalUrl);
            }
            catch (Exception ex)
            {
                if (ex.Message == "The shortened URL has expired.")
                {
                    return BadRequest(new { error = "This URL has expired. Please create a new one." });
                }
                return NotFound(new { error = ex.Message });
            }
        }

        // GET /analytics/{code}
        [HttpGet("analytics/{code}")]
        public async Task<IActionResult> GetAnalytics(string code)
        {
            try
            {
                var analytics = await _urlShortenerService.GetAnalyticsAsync(code);

                if (analytics == null)
                {
                    return NotFound(new { error = "No analytics found for the given short code." });
                }

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
