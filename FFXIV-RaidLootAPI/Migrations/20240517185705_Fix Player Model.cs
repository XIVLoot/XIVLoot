using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIV_RaidLootAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixPlayerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Statics_StaticId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "staticUUID",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "StaticId",
                table: "Players",
                newName: "staticId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_StaticId",
                table: "Players",
                newName: "IX_Players_staticId");

            migrationBuilder.AlterColumn<int>(
                name: "staticId",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Statics_staticId",
                table: "Players",
                column: "staticId",
                principalTable: "Statics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Statics_staticId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "staticId",
                table: "Players",
                newName: "StaticId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_staticId",
                table: "Players",
                newName: "IX_Players_StaticId");

            migrationBuilder.AlterColumn<int>(
                name: "StaticId",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "staticUUID",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Statics_StaticId",
                table: "Players",
                column: "StaticId",
                principalTable: "Statics",
                principalColumn: "Id");
        }
    }
}
