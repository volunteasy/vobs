﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Volunteasy.Core.Data;

#nullable disable

namespace Volunteasy.Api.Migrations
{
    [DbContext(typeof(Data))]
    partial class DataModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Volunteasy.Core.Model.Address", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AddressName")
                        .HasColumnType("text");

                    b.Property<string>("AddressNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("CoordinateX")
                        .HasColumnType("real");

                    b.Property<float>("CoordinateY")
                        .HasColumnType("real");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Beneficiary", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<long>("AddressId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("MemberSince")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .IsUnique();

                    b.HasIndex("OrganizationId");

                    b.HasIndex("Document", "BirthDate", "OrganizationId")
                        .IsUnique();

                    b.ToTable("Beneficiaries");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Benefit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AssistedId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ClaimedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("DistributionId")
                        .HasColumnType("bigint");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<long?>("Position")
                        .HasColumnType("bigint");

                    b.Property<int?>("RevokedReason")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssistedId");

                    b.HasIndex("DistributionId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Benefits");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.BenefitItem", b =>
                {
                    b.Property<long>("BenefitId")
                        .HasColumnType("bigint");

                    b.Property<long>("ResourceId")
                        .HasColumnType("bigint");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<long>("StockMovementId")
                        .HasColumnType("bigint");

                    b.HasKey("BenefitId", "ResourceId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("ResourceId");

                    b.HasIndex("StockMovementId")
                        .IsUnique();

                    b.ToTable("BenefitItems");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Distribution", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("Canceled")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndsAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MaxBenefits")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("StartsAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Distributions");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Membership", b =>
                {
                    b.Property<long>("MemberId")
                        .HasColumnType("bigint");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("MemberSince")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("MemberId", "OrganizationId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("Role");

                    b.HasIndex("Status");

                    b.ToTable("Memberships");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Organization", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AddressId")
                        .HasColumnType("bigint");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("character varying(14)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .IsUnique();

                    b.HasIndex("Document")
                        .IsUnique();

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Resource", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.StockMovement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<long?>("ImportId")
                        .HasColumnType("bigint");

                    b.Property<long>("OrganizationId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<long>("ResourceId")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Date");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("ResourceId");

                    b.HasIndex("Type");

                    b.ToTable("StockMovements");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("AddressId")
                        .HasColumnType("bigint");

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExternalId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("PhoneAddress")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("Document")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Beneficiary", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Address", "Address")
                        .WithOne()
                        .HasForeignKey("Volunteasy.Core.Model.Beneficiary", "AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Volunteasy.Core.Model.Organization", null)
                        .WithMany("Beneficiaries")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Benefit", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Beneficiary", null)
                        .WithMany("Benefits")
                        .HasForeignKey("AssistedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Volunteasy.Core.Model.Distribution", "Distribution")
                        .WithMany("Benefits")
                        .HasForeignKey("DistributionId");

                    b.HasOne("Volunteasy.Core.Model.Organization", null)
                        .WithMany("Benefits")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Distribution");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.BenefitItem", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Benefit", null)
                        .WithMany("Items")
                        .HasForeignKey("BenefitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Volunteasy.Core.Model.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Volunteasy.Core.Model.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Volunteasy.Core.Model.StockMovement", "StockMovement")
                        .WithOne()
                        .HasForeignKey("Volunteasy.Core.Model.BenefitItem", "StockMovementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");

                    b.Navigation("StockMovement");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Distribution", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Organization", null)
                        .WithMany("Distributions")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Membership", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.User", null)
                        .WithMany("Memberships")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Volunteasy.Core.Model.Organization", null)
                        .WithMany("Memberships")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Organization", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Address", "Address")
                        .WithOne()
                        .HasForeignKey("Volunteasy.Core.Model.Organization", "AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Resource", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Organization", null)
                        .WithMany("Resources")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Volunteasy.Core.Model.StockMovement", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Volunteasy.Core.Model.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.User", b =>
                {
                    b.HasOne("Volunteasy.Core.Model.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.Navigation("Address");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Beneficiary", b =>
                {
                    b.Navigation("Benefits");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Benefit", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Distribution", b =>
                {
                    b.Navigation("Benefits");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.Organization", b =>
                {
                    b.Navigation("Beneficiaries");

                    b.Navigation("Benefits");

                    b.Navigation("Distributions");

                    b.Navigation("Memberships");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("Volunteasy.Core.Model.User", b =>
                {
                    b.Navigation("Memberships");
                });
#pragma warning restore 612, 618
        }
    }
}
