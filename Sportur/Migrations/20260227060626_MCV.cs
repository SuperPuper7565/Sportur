using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class MCV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BicycleVariants_BicycleModels_BicycleModelId",
                table: "BicycleVariants");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleColorId",
                table: "BicycleVariants");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleModelId_BicycleColorId_FrameSize",
                table: "BicycleVariants");

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleColorId_FrameSize",
                table: "BicycleVariants",
                columns: new[] { "BicycleColorId", "FrameSize" },
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BicycleVariants_BicycleModels_BicycleModelId",
                table: "BicycleVariants");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleColorId_FrameSize",
                table: "BicycleVariants");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleModelId",
                table: "BicycleVariants");

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleColorId",
                table: "BicycleVariants",
                column: "BicycleColorId");

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleModelId_BicycleColorId_FrameSize",
                table: "BicycleVariants",
                columns: new[] { "BicycleModelId", "BicycleColorId", "FrameSize" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BicycleVariants_BicycleModels_BicycleModelId",
                table: "BicycleVariants",
                column: "BicycleModelId",
                principalTable: "BicycleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
