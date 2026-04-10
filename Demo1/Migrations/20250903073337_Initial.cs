using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo1.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "__SeedHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DateApplied = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___SeedHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Identification = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Hidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsProtected = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainOutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Version = table.Column<short>(type: "INTEGER", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OccurredUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CorrelationId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CausationId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TraceId = table.Column<string>(type: "TEXT", nullable: true),
                    ParentSpanId = table.Column<string>(type: "TEXT", nullable: true),
                    TraceFlags = table.Column<int>(type: "INTEGER", nullable: true),
                    TraceState = table.Column<string>(type: "TEXT", nullable: true),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Attempts = table.Column<short>(type: "INTEGER", nullable: false),
                    DoNotProcessBeforeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClaimedUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClaimedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsPoisoned = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
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
                    ReceivedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Attempts = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Version = table.Column<short>(type: "INTEGER", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OccurredUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CorrelationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CausationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    TraceId = table.Column<string>(type: "TEXT", nullable: true),
                    ParentSpanId = table.Column<string>(type: "TEXT", nullable: true),
                    TraceFlags = table.Column<int>(type: "INTEGER", nullable: true),
                    TraceState = table.Column<string>(type: "TEXT", nullable: true),
                    ProcessedUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Attempts = table.Column<short>(type: "INTEGER", nullable: false),
                    DoNotProcessBeforeUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClaimedUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClaimedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsPoisoned = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationOutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Alias = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Alias);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blogs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Alias",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Desciption = table.Column<string>(type: "TEXT", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plants_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Alias",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Alias",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: true),
                    RefreshToken = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    AuthenticationMode = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Avatar = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    IsConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActivationCode = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PasswordRedefineCode_CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PasswordRedefineCode_Hash = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    RefreshTokenValidity = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Alias",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Body = table.Column<string>(type: "TEXT", maxLength: 4096, nullable: false),
                    PublishingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UniqueInApplication = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    UniqueInTenant = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Alias",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolesToClaims",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesToClaims", x => new { x.ClaimId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RolesToClaims_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolesToClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersToClaims",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Access = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersToClaims", x => new { x.ClaimId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UsersToClaims_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersToClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersToRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersToRoles", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UsersToRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersToRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationOptions_Key_TenantId_Username",
                table: "ApplicationOptions",
                columns: new[] { "Key", "TenantId", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_TenantId",
                table: "Blogs",
                column: "TenantId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Plants_TenantId",
                table: "Plants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BlogId",
                table: "Posts",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TenantId",
                table: "Posts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId",
                table: "Roles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesToClaims_RoleId",
                table: "RolesToClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToClaims_UserId",
                table: "UsersToClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToRoles_UserId",
                table: "UsersToRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__SeedHistory");

            migrationBuilder.DropTable(
                name: "ApplicationOptions");

            migrationBuilder.DropTable(
                name: "DomainOutboxMessages");

            migrationBuilder.DropTable(
                name: "InboxMessages");

            migrationBuilder.DropTable(
                name: "IntegrationOutboxMessages");

            migrationBuilder.DropTable(
                name: "Plants");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "RolesToClaims");

            migrationBuilder.DropTable(
                name: "UsersToClaims");

            migrationBuilder.DropTable(
                name: "UsersToRoles");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
