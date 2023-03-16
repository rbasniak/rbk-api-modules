using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Demo2.Database.Relational
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    DateApplied = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___SeedHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChageRequestSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChageRequestSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequestPriorities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestPriorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequestTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FicCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FicCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Domain = table.Column<string>(type: "text", nullable: true),
                    Repository = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UnId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Platforms_Uns_UnId",
                        column: x => x.UnId,
                        principalTable: "Uns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceNumber = table.Column<string>(type: "text", nullable: true),
                    RequestedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PriorityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Justification = table.Column<string>(type: "text", nullable: true),
                    StateId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusSgm = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    Complexity = table.Column<double>(type: "double precision", nullable: false),
                    Prioritization_Gravity = table.Column<int>(type: "integer", nullable: true),
                    Prioritization_Urgency = table.Column<int>(type: "integer", nullable: true),
                    Prioritization_Tendency = table.Column<int>(type: "integer", nullable: true),
                    Resource = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CheckedBy = table.Column<string>(type: "text", nullable: true),
                    CurrentOwner = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DesiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InternalNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_ChageRequestSources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "ChageRequestSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_ChangeRequestPriorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "ChangeRequestPriorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_ChangeRequestTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ChangeRequestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    Filename = table.Column<string>(type: "text", nullable: true),
                    ChangeRequestId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_AttachmentTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AttachmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_ChangeRequests_ChangeRequestId1",
                        column: x => x.ChangeRequestId1,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequestToDisciplines",
                columns: table => new
                {
                    ChangeRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeRequestId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestToDisciplines", x => new { x.ChangeRequestId, x.DisciplineId });
                    table.ForeignKey(
                        name: "FK_ChangeRequestToDisciplines_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequestToDisciplines_ChangeRequests_ChangeRequestId1",
                        column: x => x.ChangeRequestId1,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChangeRequestToDisciplines_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SindotecUrl = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "DocumentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvidenceAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    AdditionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    Filename = table.Column<string>(type: "text", nullable: true),
                    Commentary = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidenceAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvidenceAttachments_AttachmentTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AttachmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvidenceAttachments_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fic_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fic_FicCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FicCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StateChangeEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusName = table.Column<string>(type: "text", nullable: true),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusHistory = table.Column<string>(type: "text", nullable: true),
                    User = table.Column<string>(type: "text", nullable: true),
                    NewOwner = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateChangeEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateChangeEvent_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ChangeRequestId",
                table: "Attachments",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ChangeRequestId1",
                table: "Attachments",
                column: "ChangeRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TypeId",
                table: "Attachments",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_PlatformId",
                table: "ChangeRequests",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_PriorityId",
                table: "ChangeRequests",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_SourceId",
                table: "ChangeRequests",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_StateId",
                table: "ChangeRequests",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_TypeId",
                table: "ChangeRequests",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestToDisciplines_ChangeRequestId1",
                table: "ChangeRequestToDisciplines",
                column: "ChangeRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestToDisciplines_DisciplineId",
                table: "ChangeRequestToDisciplines",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CategoryId",
                table: "Documents",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ChangeRequestId",
                table: "Documents",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenceAttachments_ChangeRequestId",
                table: "EvidenceAttachments",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenceAttachments_TypeId",
                table: "EvidenceAttachments",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fic_CategoryId",
                table: "Fic",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Fic_ChangeRequestId",
                table: "Fic",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_UnId",
                table: "Platforms",
                column: "UnId");

            migrationBuilder.CreateIndex(
                name: "IX_StateChangeEvent_ChangeRequestId",
                table: "StateChangeEvent",
                column: "ChangeRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__SeedHistory");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "ChangeRequestToDisciplines");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "EvidenceAttachments");

            migrationBuilder.DropTable(
                name: "Fic");

            migrationBuilder.DropTable(
                name: "StateChangeEvent");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "DocumentCategories");

            migrationBuilder.DropTable(
                name: "AttachmentTypes");

            migrationBuilder.DropTable(
                name: "FicCategories");

            migrationBuilder.DropTable(
                name: "ChangeRequests");

            migrationBuilder.DropTable(
                name: "ChageRequestSources");

            migrationBuilder.DropTable(
                name: "ChangeRequestPriorities");

            migrationBuilder.DropTable(
                name: "ChangeRequestTypes");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Uns");
        }
    }
}
