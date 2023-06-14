using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveActiveFieldFromMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Memberships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Memberships",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
