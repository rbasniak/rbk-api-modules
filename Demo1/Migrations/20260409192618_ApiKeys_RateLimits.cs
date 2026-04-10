using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo1.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeys_RateLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BurstLimit",
                table: "ApiKeys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 600);

            migrationBuilder.AddColumn<int>(
                name: "RequestsPerMinute",
                table: "ApiKeys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 600);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BurstLimit",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "RequestsPerMinute",
                table: "ApiKeys");
        }
    }
}
