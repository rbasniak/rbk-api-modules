using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo3.Databse.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuthModeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthenticationMode",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticationMode",
                table: "Users");
        }
    }
}
