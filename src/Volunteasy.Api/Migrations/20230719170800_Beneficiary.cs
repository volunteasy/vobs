using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Volunteasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class Beneficiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Organizations",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Document = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MemberSince = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Slug",
                table: "Organizations",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Benefits_AssistedId",
                table: "Benefits",
                column: "AssistedId");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitItems_OrganizationId",
                table: "BenefitItems",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_AddressId",
                table: "Beneficiaries",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_Document_BirthDate_OrganizationId",
                table: "Beneficiaries",
                columns: new[] { "Document", "BirthDate", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_OrganizationId",
                table: "Beneficiaries",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BenefitItems_Organizations_OrganizationId",
                table: "BenefitItems",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Benefits_Beneficiaries_AssistedId",
                table: "Benefits",
                column: "AssistedId",
                principalTable: "Beneficiaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BenefitItems_Organizations_OrganizationId",
                table: "BenefitItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Benefits_Beneficiaries_AssistedId",
                table: "Benefits");

            migrationBuilder.DropTable(
                name: "Beneficiaries");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_Slug",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Benefits_AssistedId",
                table: "Benefits");

            migrationBuilder.DropIndex(
                name: "IX_BenefitItems_OrganizationId",
                table: "BenefitItems");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Organizations");
        }
    }
}
