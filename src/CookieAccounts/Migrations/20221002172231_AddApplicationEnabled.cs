using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookieAccounts.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>("enabled", "applications", type: "boolean", nullable: false,defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("enabled", "applications");
        }
    }
}
