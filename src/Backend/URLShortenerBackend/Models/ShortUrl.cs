namespace URLShortenerBackend.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; } = null!;

        public string ShortCode { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
