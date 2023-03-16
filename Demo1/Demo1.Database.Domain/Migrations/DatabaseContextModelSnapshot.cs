﻿// <auto-generated />
using System;
using Demo1.Database.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Demo1.Database.Domain.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Demo1.Models.Domain.Demo.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Demo.Blog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Demo.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("BlogId")
                        .HasColumnType("uuid");

                    b.Property<string>("Body")
                        .HasMaxLength(4096)
                        .HasColumnType("character varying(4096)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("PublishingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BlogId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Folders.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Extension")
                        .HasColumnType("text");

                    b.Property<Guid>("FolderId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Size")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("File");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Folders.Folder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Folder");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Plant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Desciption")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Plant");
                });

            modelBuilder.Entity("rbkApiModules.Comments.Core.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<string>("TenantId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("TenantId");

                    b.ToTable("Comments", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Commons.Core.Auditing.TraceLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CommandId")
                        .HasColumnType("uuid");

                    b.Property<string>("CommandName")
                        .HasColumnType("text");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("Payload")
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TraceLog");
                });

            modelBuilder.Entity("rbkApiModules.Commons.Relational.SeedHistory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateApplied")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("__SeedHistory", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Faqs.Core.Faq", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Answer")
                        .HasColumnType("text");

                    b.Property<string>("Question")
                        .HasColumnType("text");

                    b.Property<string>("Tag")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Faqs", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Claim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("Hidden")
                        .HasColumnType("boolean");

                    b.Property<string>("Identification")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<bool>("IsProtected")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Claims", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("TenantId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.RoleToClaim", b =>
                {
                    b.Property<Guid>("ClaimId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("ClaimId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("RolesToClaims", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Tenant", b =>
                {
                    b.Property<string>("Alias")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Metadata")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Alias");

                    b.ToTable("Tenants", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ActivationCode")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Avatar")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Metadata")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(4096)
                        .HasColumnType("character varying(4096)");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("RefreshTokenValidity")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TenantId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.UserToClaim", b =>
                {
                    b.Property<Guid>("ClaimId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("Access")
                        .HasColumnType("integer");

                    b.HasKey("ClaimId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersToClaims", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.UserToRole", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersToRoles", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Notifications.Core.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Body")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("Category")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Link")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Route")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("User")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("Notifications", (string)null);
                });

            modelBuilder.Entity("Demo1.Models.Domain.Demo.Post", b =>
                {
                    b.HasOne("Demo1.Models.Domain.Demo.Author", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Demo1.Models.Domain.Demo.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Blog");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Folders.File", b =>
                {
                    b.HasOne("Demo1.Models.Domain.Folders.Folder", "Folder")
                        .WithMany("Files")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Folders.Folder", b =>
                {
                    b.HasOne("Demo1.Models.Domain.Folders.Folder", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Plant", b =>
                {
                    b.HasOne("rbkApiModules.Identity.Core.Tenant", null)
                        .WithMany()
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("rbkApiModules.Comments.Core.Comment", b =>
                {
                    b.HasOne("rbkApiModules.Comments.Core.Comment", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("rbkApiModules.Identity.Core.Tenant", null)
                        .WithMany()
                        .HasForeignKey("TenantId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("rbkApiModules.Faqs.Core.Faq", b =>
                {
                    b.HasOne("rbkApiModules.Identity.Core.Tenant", null)
                        .WithMany()
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Role", b =>
                {
                    b.HasOne("rbkApiModules.Identity.Core.Tenant", null)
                        .WithMany()
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.RoleToClaim", b =>
                {
                    b.HasOne("rbkApiModules.Identity.Core.Claim", "Claim")
                        .WithMany("Roles")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("rbkApiModules.Identity.Core.Role", "Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Claim");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.User", b =>
                {
                    b.HasOne("rbkApiModules.Identity.Core.Tenant", null)
                        .WithMany()
                        .HasForeignKey("TenantId");

                    b.OwnsOne("rbkApiModules.Identity.Core.PasswordRedefineCode", "PasswordRedefineCode", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime?>("CreationDate")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<string>("Hash")
                                .HasMaxLength(1024)
                                .HasColumnType("character varying(1024)");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("PasswordRedefineCode");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.UserToClaim", b =>
                {
                    b.HasOne("rbkApiModules.Identity.Core.Claim", "Claim")
                        .WithMany("Users")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("rbkApiModules.Identity.Core.User", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Claim");

                    b.Navigation("User");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.UserToRole", b =>
                {
                    b.HasOne("rbkApiModules.Identity.Core.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("rbkApiModules.Identity.Core.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Demo.Author", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Demo.Blog", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("Demo1.Models.Domain.Folders.Folder", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Files");
                });

            modelBuilder.Entity("rbkApiModules.Comments.Core.Comment", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Claim", b =>
                {
                    b.Navigation("Roles");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Role", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.User", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
