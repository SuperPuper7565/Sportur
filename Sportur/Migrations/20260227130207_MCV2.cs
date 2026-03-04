using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class MCV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BicycleVariants_BicycleModels_BicycleModelId",
                table: "BicycleVariants");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleModelId",
                table: "BicycleVariants");

            migrationBuilder.DropColumn(
                name: "BicycleModelId",
                table: "BicycleVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BicycleModelId",
                table: "BicycleVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleModelId",
                table: "BicycleVariants",
                column: "BicycleModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_BicycleVariants_BicycleModels_BicycleModelId",
                table: "BicycleVariants",
                column: "BicycleModelId",
                principalTable: "BicycleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
