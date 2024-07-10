using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addedbookinfoforgearacq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAcquiredFromBook",
                table: "GearAcquisitionTimestamps",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAcquiredFromBook",
                table: "GearAcquisitionTimestamps");
        }
    }
}
