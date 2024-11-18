﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URLShortenerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiresAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "ShortUrls",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "ShortUrls");
        }
    }
}
