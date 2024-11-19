using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using URLShortenerBackend.Models;

namespace URLShortenerBackend.Data;

public partial class UrlShortenerDbContext : DbContext
{
    public UrlShortenerDbContext()
    {
    }

    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ShortUrl> ShortUrls { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("url");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp");
            entity.Property(e => e.ExpiresAt)
                .HasDefaultValueSql("'''0000-00-00 00:00:00'''")
                .HasColumnType("timestamp");
            entity.Property(e => e.OriginalUrl).HasMaxLength(2083);
            entity.Property(e => e.ShortCode).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
