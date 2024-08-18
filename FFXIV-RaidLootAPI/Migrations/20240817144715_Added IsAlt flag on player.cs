using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsAltflagonplayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAlt",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlt",
                table: "Players");
        }
    }
}
