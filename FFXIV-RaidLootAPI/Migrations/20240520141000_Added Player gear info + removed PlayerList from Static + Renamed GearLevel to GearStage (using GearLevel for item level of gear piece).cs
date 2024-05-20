using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayergearinforemovedPlayerListfromStaticRenamedGearLeveltoGearStageusingGearLevelforitemlevelofgearpiece : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Statics_staticId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_staticId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "GearItemLevel",
                table: "Gears",
                newName: "GearStage");

            migrationBuilder.AddColumn<int>(
                name: "BiSNecklaceGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisBraceletGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisCoatGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisEarringGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisFeetGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisHandGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisHeadGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisLeftRingGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisLegGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisRightRingGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BisWeaponGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurBraceletGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurCoatGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurEarringGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurFeetGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurHandGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurHeadGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurLeftRingGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurLegGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurNecklaceGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurRightRingGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurWeaponGearId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EtroBiS",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BiSNecklaceGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisBraceletGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisCoatGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisEarringGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisFeetGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisHandGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisHeadGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisLeftRingGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisLegGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisRightRingGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BisWeaponGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurBraceletGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurCoatGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurEarringGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurFeetGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurHandGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurHeadGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurLeftRingGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurLegGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurNecklaceGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurRightRingGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurWeaponGearId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "EtroBiS",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "GearStage",
                table: "Gears",
                newName: "GearItemLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Players_staticId",
                table: "Players",
                column: "staticId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Statics_staticId",
                table: "Players",
                column: "staticId",
                principalTable: "Statics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
