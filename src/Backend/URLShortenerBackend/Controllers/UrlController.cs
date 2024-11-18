using Microsoft.AspNetCore.Mvc;
using URLShortenerBackend.Services;

namespace URLShortenerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly UrlShortenerService _urlShortenerService;

        public UrlController(UrlShortenerService urlShortenerService)
        {
            _urlShortenerService = urlShortenerService;
        }

        // POST /shorten
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {
            try
            {
                var shortCode = await _urlShortenerService.CreateShortUrlAsync(originalUrl);
                var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";
                return Ok(shortUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

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
