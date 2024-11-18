﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using URLShortenerBackend.Data;

#nullable disable

namespace URLShortenerBackend.Migrations
{
    [DbContext(typeof(UrlShortenerDbContext))]
    partial class UrlShortenerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("URLShortenerBackend.Models.ShortUrl", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("OriginalUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ShortUrls", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
