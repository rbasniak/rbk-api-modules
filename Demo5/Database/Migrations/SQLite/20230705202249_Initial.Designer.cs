﻿// <auto-generated />
using System;
using Demo5;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Demo5.Migrations.SQLite
{
    [DbContext(typeof(TestingDatabaseContext))]
    [Migration("20230705202249_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.8");

            modelBuilder.Entity("rbkApiModules.Commons.Relational.SeedHistory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateApplied")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("__SeedHistory", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Claim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Hidden")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Identification")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsProtected")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Claims", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("TenantId")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.RoleToClaim", b =>
                {
                    b.Property<Guid>("ClaimId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("ClaimId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("RolesToClaims", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.Tenant", b =>
                {
                    b.Property<string>("Alias")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("Metadata")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Alias");

                    b.ToTable("Tenants", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ActivationCode")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("AuthenticationMode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Avatar")
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("TEXT");

                    b.Property<string>("Metadata")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasMaxLength(4096)
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RefreshTokenValidity")
                        .HasColumnType("TEXT");

                    b.Property<string>("TenantId")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.UserToClaim", b =>
                {
                    b.Property<Guid>("ClaimId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Access")
                        .HasColumnType("INTEGER");

                    b.HasKey("ClaimId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersToClaims", (string)null);
                });

            modelBuilder.Entity("rbkApiModules.Identity.Core.UserToRole", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersToRoles", (string)null);
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
                                .HasColumnType("TEXT");

                            b1.Property<DateTime?>("CreationDate")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Hash")
                                .HasMaxLength(1024)
                                .HasColumnType("TEXT");

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
