using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovedgearlistfromPlayerandaddedAvgILeveltogear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gears_Players_PlayersId",
                table: "Gears");

            migrationBuilder.DropIndex(
                name: "IX_Gears_PlayersId",
                table: "Gears");

            migrationBuilder.DropColumn(
                name: "PlayersId",
                table: "Gears");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayersId",
                table: "Gears",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gears_PlayersId",
                table: "Gears",
                column: "PlayersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gears_Players_PlayersId",
                table: "Gears",
                column: "PlayersId",
                principalTable: "Players",
                principalColumn: "Id");
        }
    }
}
