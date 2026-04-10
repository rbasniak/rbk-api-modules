using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo1.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeysAndClaimFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowApiKeyUsage",
                table: "Claims",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    KeyHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    KeyPrefix = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeysToClaims",
                columns: table => new
                {
                    ApiKeyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Access = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeysToClaims", x => new { x.ClaimId, x.ApiKeyId });
                    table.ForeignKey(
                        name: "FK_ApiKeysToClaims_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalTable: "ApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiKeysToClaims_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyUsageByDay",
                columns: table => new
                {
                    ApiKeyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyUsageByDay", x => new { x.ApiKeyId, x.Date });
                    table.ForeignKey(
                        name: "FK_ApiKeyUsageByDay_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalTable: "ApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_KeyHash",
                table: "ApiKeys",
                column: "KeyHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeysToClaims_ApiKeyId",
                table: "ApiKeysToClaims",
                column: "ApiKeyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeysToClaims");

            migrationBuilder.DropTable(
                name: "ApiKeyUsageByDay");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "AllowApiKeyUsage",
                table: "Claims");
        }
    }
}
