using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace rbkApiModules.Analytics.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Identity = table.Column<string>(maxLength: 64, nullable: true),
                    Username = table.Column<string>(maxLength: 64, nullable: true),
                    RemoteIpAddress = table.Column<string>(maxLength: 32, nullable: true),
                    IsHttps = table.Column<bool>(nullable: false),
                    Method = table.Column<string>(maxLength: 16, nullable: true),
                    Path = table.Column<string>(maxLength: 1024, nullable: true),
                    Response = table.Column<int>(nullable: false),
                    UserAgent = table.Column<string>(maxLength: 512, nullable: true),
                    Duration = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
