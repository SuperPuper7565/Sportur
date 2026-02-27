using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class RefactorVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BicycleVariants_BicycleSizes_BicycleSizeId",
                table: "BicycleVariants");

            migrationBuilder.DropTable(
                name: "BicycleSizes");

            migrationBuilder.DropIndex(
                name: "IX_WholesalePrices_BicycleVariantId",
                table: "WholesalePrices");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleModelId_BicycleColorId_BicycleSizeId",
                table: "BicycleVariants");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleSizeId",
                table: "BicycleVariants");

            migrationBuilder.DropColumn(
                name: "BicycleSizeId",
                table: "BicycleVariants");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "BicycleVariants");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "Orders",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "FrameSize",
                table: "BicycleVariants",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOverride",
                table: "BicycleVariants",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "BicycleModels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_WholesalePrices_BicycleVariantId_UserId",
                table: "WholesalePrices",
                columns: new[] { "BicycleVariantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleModelId_BicycleColorId_FrameSize",
                table: "BicycleVariants",
                columns: new[] { "BicycleModelId", "BicycleColorId", "FrameSize" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WholesalePrices_BicycleVariantId_UserId",
                table: "WholesalePrices");

            migrationBuilder.DropIndex(
                name: "IX_BicycleVariants_BicycleModelId_BicycleColorId_FrameSize",
                table: "BicycleVariants");

            migrationBuilder.DropColumn(
                name: "FrameSize",
                table: "BicycleVariants");

            migrationBuilder.DropColumn(
                name: "PriceOverride",
                table: "BicycleVariants");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "BicycleModels");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Orders",
                newName: "OrderDate");

            migrationBuilder.AddColumn<int>(
                name: "BicycleSizeId",
                table: "BicycleVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "BicycleVariants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "BicycleSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BicycleModelId = table.Column<int>(type: "int", nullable: false),
                    FrameSize = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BicycleSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BicycleSizes_BicycleModels_BicycleModelId",
                        column: x => x.BicycleModelId,
                        principalTable: "BicycleModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WholesalePrices_BicycleVariantId",
                table: "WholesalePrices",
                column: "BicycleVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleModelId_BicycleColorId_BicycleSizeId",
                table: "BicycleVariants",
                columns: new[] { "BicycleModelId", "BicycleColorId", "BicycleSizeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleSizeId",
                table: "BicycleVariants",
                column: "BicycleSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_BicycleSizes_BicycleModelId",
                table: "BicycleSizes",
                column: "BicycleModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_BicycleVariants_BicycleSizes_BicycleSizeId",
                table: "BicycleVariants",
                column: "BicycleSizeId",
                principalTable: "BicycleSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
