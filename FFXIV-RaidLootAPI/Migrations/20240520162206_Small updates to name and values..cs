using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class Smallupdatestonameandvalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BiSNecklaceGearId",
                table: "Players",
                newName: "BisNecklaceGearId");

            migrationBuilder.RenameColumn(
                name: "CurLegGearId",
                table: "Players",
                newName: "Job");

            migrationBuilder.RenameColumn(
                name: "CurHandGearId",
                table: "Players",
                newName: "CurLegsGearId");

            migrationBuilder.RenameColumn(
                name: "CurEarringGearId",
                table: "Players",
                newName: "CurHandsGearId");

            migrationBuilder.RenameColumn(
                name: "CurCoatGearId",
                table: "Players",
                newName: "CurEarringsGearId");

            migrationBuilder.RenameColumn(
                name: "CurBraceletGearId",
                table: "Players",
                newName: "CurBraceletsGearId");

            migrationBuilder.RenameColumn(
                name: "BisLegGearId",
                table: "Players",
                newName: "CurBodyGearId");

            migrationBuilder.RenameColumn(
                name: "BisHandGearId",
                table: "Players",
                newName: "BisLegsGearId");

            migrationBuilder.RenameColumn(
                name: "BisEarringGearId",
                table: "Players",
                newName: "BisHandsGearId");

            migrationBuilder.RenameColumn(
                name: "BisCoatGearId",
                table: "Players",
                newName: "BisEarringsGearId");

            migrationBuilder.RenameColumn(
                name: "BisBraceletGearId",
                table: "Players",
                newName: "BisBraceletsGearId");

            migrationBuilder.AddColumn<int>(
                name: "BisBodyGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BisBodyGearId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "BisNecklaceGearId",
                table: "Players",
                newName: "BiSNecklaceGearId");

            migrationBuilder.RenameColumn(
                name: "Job",
                table: "Players",
                newName: "CurLegGearId");

            migrationBuilder.RenameColumn(
                name: "CurLegsGearId",
                table: "Players",
                newName: "CurHandGearId");

            migrationBuilder.RenameColumn(
                name: "CurHandsGearId",
                table: "Players",
                newName: "CurEarringGearId");

            migrationBuilder.RenameColumn(
                name: "CurEarringsGearId",
                table: "Players",
                newName: "CurCoatGearId");

            migrationBuilder.RenameColumn(
                name: "CurBraceletsGearId",
                table: "Players",
                newName: "CurBraceletGearId");

            migrationBuilder.RenameColumn(
                name: "CurBodyGearId",
                table: "Players",
                newName: "BisLegGearId");

            migrationBuilder.RenameColumn(
                name: "BisLegsGearId",
                table: "Players",
                newName: "BisHandGearId");

            migrationBuilder.RenameColumn(
                name: "BisHandsGearId",
                table: "Players",
                newName: "BisEarringGearId");

            migrationBuilder.RenameColumn(
                name: "BisEarringsGearId",
                table: "Players",
                newName: "BisCoatGearId");

            migrationBuilder.RenameColumn(
                name: "BisBraceletsGearId",
                table: "Players",
                newName: "BisBraceletGearId");
        }
    }
}
