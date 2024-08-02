using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedstaticownerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ownerId",
                table: "Statics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ownerId",
                table: "Statics");
        }
    }
}
