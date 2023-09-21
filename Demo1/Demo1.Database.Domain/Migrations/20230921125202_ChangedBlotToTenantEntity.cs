using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo1.Database.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBlotToTenantEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Posts",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Posts",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Blogs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TenantId",
                table: "Posts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_TenantId",
                table: "Blogs",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Tenants_TenantId",
                table: "Blogs",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Alias");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Tenants_TenantId",
                table: "Posts",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Alias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Tenants_TenantId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Tenants_TenantId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_TenantId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_TenantId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Posts",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Posts",
                type: "text",
                nullable: true);
        }
    }
}
