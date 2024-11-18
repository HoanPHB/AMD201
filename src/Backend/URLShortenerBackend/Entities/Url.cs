using System;
using System.Collections.Generic;

namespace URLShortenerBackend.Entities;

public partial class Url
{
    public int Id { get; set; }

    public string OriginalUrl { get; set; } = null!;

    public string ShortCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}
