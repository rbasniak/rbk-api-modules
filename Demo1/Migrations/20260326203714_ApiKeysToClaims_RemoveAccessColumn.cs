using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo1.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeysToClaims_RemoveAccessColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Access",
                table: "ApiKeysToClaims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Access",
                table: "ApiKeysToClaims",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
