using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRolefromplayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
