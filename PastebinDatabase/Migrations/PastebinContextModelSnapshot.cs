﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PastebinDatabase.Context;

#nullable disable

namespace PastebinDatabase.Migrations
{
    [DbContext(typeof(PastebinContext))]
    partial class PastebinContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PastebinDatabase.Entities.PasteEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id");

                    b.ToTable("Pastes", (string)null);
                });

            modelBuilder.Entity("PastebinDatabase.Entities.PasteMetaEntity", b =>
                {
                    b.Property<Guid>("PasteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("BurnOnRead")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("PasswordProtected")
                        .HasColumnType("bit");

                    b.Property<int>("Visibility")
                        .HasColumnType("int");

                    b.HasKey("PasteId");

                    b.ToTable("Pastes_Metas", (string)null);
                });

            modelBuilder.Entity("PastebinDatabase.Entities.PastePasswordEntity", b =>
                {
                    b.Property<Guid>("PasteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PasteId");

                    b.ToTable("Pastes_Passwords", (string)null);
                });

            modelBuilder.Entity("PastebinDatabase.Entities.PasteMetaEntity", b =>
                {
                    b.HasOne("PastebinDatabase.Entities.PasteEntity", "Paste")
                        .WithOne("Meta")
                        .HasForeignKey("PastebinDatabase.Entities.PasteMetaEntity", "PasteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Paste");
                });

            modelBuilder.Entity("PastebinDatabase.Entities.PastePasswordEntity", b =>
                {
                    b.HasOne("PastebinDatabase.Entities.PasteMetaEntity", "Meta")
                        .WithOne("Password")
                        .HasForeignKey("PastebinDatabase.Entities.PastePasswordEntity", "PasteId");

                    b.Navigation("Meta");
                });

            modelBuilder.Entity("PastebinDatabase.Entities.PasteEntity", b =>
                {
                    b.Navigation("Meta")
                        .IsRequired();
                });

            modelBuilder.Entity("PastebinDatabase.Entities.PasteMetaEntity", b =>
                {
                    b.Navigation("Password");
                });
#pragma warning restore 612, 618
        }
    }
}
