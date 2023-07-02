using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Volunteasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQueuePosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Position",
                table: "Benefits",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Benefits");
        }
    }
}
