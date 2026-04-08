using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo2.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeys_RevokeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RevokeDate",
                table: "ApiKeys",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevokeReason",
                table: "ApiKeys",
                type: "TEXT",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RevokeDate",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "RevokeReason",
                table: "ApiKeys");
        }
    }
}
