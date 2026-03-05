using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo1.Migrations
{
    /// <inheritdoc />
    public partial class Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainOutboxMessages");

            migrationBuilder.DropTable(
                name: "InboxMessages");

            migrationBuilder.DropTable(
                name: "IntegrationOutboxMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainOutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Attempts = table.Column<short>(type: "INTEGER", nullable: false),
                    CausationId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ClaimedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    ClaimedUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CorrelationId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DoNotProcessBeforeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPoisoned = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    OccurredUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParentSpanId = table.Column<string>(type: "TEXT", nullable: true),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TraceFlags = table.Column<int>(type: "INTEGER", nullable: true),
                    TraceId = table.Column<string>(type: "TEXT", nullable: true),
                    TraceState = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainOutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    HandlerName = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Attempts = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReceivedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => new { x.EventId, x.HandlerName });
                });

            migrationBuilder.CreateTable(
                name: "IntegrationOutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Attempts = table.Column<short>(type: "INTEGER", nullable: false),
                    CausationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ClaimedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    ClaimedUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CorrelationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DoNotProcessBeforeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPoisoned = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    OccurredUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParentSpanId = table.Column<string>(type: "TEXT", nullable: true),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TraceFlags = table.Column<int>(type: "INTEGER", nullable: true),
                    TraceId = table.Column<string>(type: "TEXT", nullable: true),
                    TraceState = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationOutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainOutboxMessages_CreatedUtc",
                table: "DomainOutboxMessages",
                column: "CreatedUtc",
                filter: "\"ProcessedUtc\" IS NULL AND \"IsPoisoned\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_DomainOutboxMessages_DoNotProcessBeforeUtc",
                table: "DomainOutboxMessages",
                column: "DoNotProcessBeforeUtc");

            migrationBuilder.CreateIndex(
                name: "IX_DomainOutboxMessages_IsPoisoned",
                table: "DomainOutboxMessages",
                column: "IsPoisoned");

            migrationBuilder.CreateIndex(
                name: "IX_DomainOutboxMessages_ProcessedUtc",
                table: "DomainOutboxMessages",
                column: "ProcessedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_ProcessedUtc",
                table: "InboxMessages",
                column: "ProcessedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationOutboxMessages_CreatedUtc",
                table: "IntegrationOutboxMessages",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationOutboxMessages_DoNotProcessBeforeUtc",
                table: "IntegrationOutboxMessages",
                column: "DoNotProcessBeforeUtc");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationOutboxMessages_IsPoisoned",
                table: "IntegrationOutboxMessages",
                column: "IsPoisoned");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationOutboxMessages_ProcessedUtc",
                table: "IntegrationOutboxMessages",
                column: "ProcessedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationOutboxMessages_TenantId_Name_Version",
                table: "IntegrationOutboxMessages",
                columns: new[] { "TenantId", "Name", "Version" });
        }
    }
}
