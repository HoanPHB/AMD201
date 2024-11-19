namespace URLShortenerBackend.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; } = null!;

        public string ShortCode { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        // New properties for analytics
        public int ClickCount { get; set; } = 0; // Default value is 0
        public DateTime? LastClickedAt { get; set; } // Last time the link was accessed
    }
}
