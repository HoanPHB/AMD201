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
            public string OriginalUrl { get; set; }
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
                Console.WriteLine($"Received request to redirect for code: {code}");

                var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(code);
                Console.WriteLine($"Redirecting to: {originalUrl}");

                return Redirect(originalUrl); // Redirect to the original URL
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during redirection: {ex.Message}");
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
