using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rbkApiModules.Demo.Migrations
{
    public partial class AddedClaimProtection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProtected",
                table: "Claims",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProtected",
                table: "Claims");
        }
    }
}
