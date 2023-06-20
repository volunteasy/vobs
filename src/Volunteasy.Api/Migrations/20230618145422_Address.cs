using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Volunteasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class Address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressName",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CoordinateX",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CoordinateY",
                table: "Organizations");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "Organizations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressName = table.Column<string>(type: "text", nullable: true),
                    AddressNumber = table.Column<string>(type: "text", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false),
                    CoordinateX = table.Column<float>(type: "real", nullable: false),
                    CoordinateY = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_AddressId",
                table: "Organizations",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_Users_MemberId",
                table: "Memberships",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Addresses_AddressId",
                table: "Organizations",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_Users_MemberId",
                table: "Memberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Addresses_AddressId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_AddressId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "AddressName",
                table: "Organizations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "CoordinateX",
                table: "Organizations",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CoordinateY",
                table: "Organizations",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
