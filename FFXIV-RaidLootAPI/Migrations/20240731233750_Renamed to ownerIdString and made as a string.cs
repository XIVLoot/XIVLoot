using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenamedtoownerIdStringandmadeasastring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ownerId",
                table: "Statics");

            migrationBuilder.AddColumn<string>(
                name: "ownerIdString",
                table: "Statics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ownerIdString",
                table: "Statics");

            migrationBuilder.AddColumn<int>(
                name: "ownerId",
                table: "Statics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
