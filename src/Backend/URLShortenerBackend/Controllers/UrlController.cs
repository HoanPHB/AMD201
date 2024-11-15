using Microsoft.AspNetCore.Mvc;

namespace URLShortenerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        // POST /shorten
        [HttpPost("shorten")]
        public IActionResult ShortenUrl([FromBody] UrlRequest request)
        {
            // Skeleton response, no actual logic yet
            return Ok(new { ShortUrl = "https://short.ly/example" });
        }

        // GET /{code}
        [HttpGet("{code}")]
        public IActionResult RedirectToOriginalUrl(string code)
        {
            // Skeleton response, no actual logic yet
            return Redirect("https://original-url-example.com");
        }
    }

    // This is a model for the URL request data (assumes an input JSON object with a single `url` property)
    public class UrlRequest
    {
        public string Url { get; set; }
    }
}
