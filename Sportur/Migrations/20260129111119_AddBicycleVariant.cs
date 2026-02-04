using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class AddBicycleVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_BicycleColors_BicycleColorId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_BicycleSizes_BicycleSizeId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WholesalePrices_BicycleSizes_BicycleSizeId",
                table: "WholesalePrices");

            migrationBuilder.DropIndex(
                name: "IX_WholesalePrices_BicycleSizeId_UserId",
                table: "WholesalePrices");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_BicycleColorId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BicycleColorId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "BicycleSizes");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "BicycleSizes");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "BicycleColors");

            migrationBuilder.RenameColumn(
                name: "BicycleSizeId",
                table: "WholesalePrices",
                newName: "BicycleVariantId");

            migrationBuilder.RenameColumn(
                name: "BicycleSizeId",
                table: "OrderItems",
                newName: "BicycleVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_BicycleSizeId",
                table: "OrderItems",
                newName: "IX_OrderItems_BicycleVariantId");

            migrationBuilder.CreateTable(
                name: "BicycleVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BicycleModelId = table.Column<int>(type: "int", nullable: false),
                    BicycleColorId = table.Column<int>(type: "int", nullable: false),
                    BicycleSizeId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BicycleVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BicycleVariants_BicycleColors_BicycleColorId",
                        column: x => x.BicycleColorId,
                        principalTable: "BicycleColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BicycleVariants_BicycleModels_BicycleModelId",
                        column: x => x.BicycleModelId,
                        principalTable: "BicycleModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BicycleVariants_BicycleSizes_BicycleSizeId",
                        column: x => x.BicycleSizeId,
                        principalTable: "BicycleSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WholesalePrices_BicycleVariantId",
                table: "WholesalePrices",
                column: "BicycleVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleColorId",
                table: "BicycleVariants",
                column: "BicycleColorId");

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleModelId_BicycleColorId_BicycleSizeId",
                table: "BicycleVariants",
                columns: new[] { "BicycleModelId", "BicycleColorId", "BicycleSizeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BicycleVariants_BicycleSizeId",
                table: "BicycleVariants",
                column: "BicycleSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_BicycleVariants_BicycleVariantId",
                table: "OrderItems",
                column: "BicycleVariantId",
                principalTable: "BicycleVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WholesalePrices_BicycleVariants_BicycleVariantId",
                table: "WholesalePrices",
                column: "BicycleVariantId",
                principalTable: "BicycleVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_BicycleVariants_BicycleVariantId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WholesalePrices_BicycleVariants_BicycleVariantId",
                table: "WholesalePrices");

            migrationBuilder.DropTable(
                name: "BicycleVariants");

            migrationBuilder.DropIndex(
                name: "IX_WholesalePrices_BicycleVariantId",
                table: "WholesalePrices");

            migrationBuilder.RenameColumn(
                name: "BicycleVariantId",
                table: "WholesalePrices",
                newName: "BicycleSizeId");

            migrationBuilder.RenameColumn(
                name: "BicycleVariantId",
                table: "OrderItems",
                newName: "BicycleSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_BicycleVariantId",
                table: "OrderItems",
                newName: "IX_OrderItems_BicycleSizeId");

            migrationBuilder.AddColumn<int>(
                name: "BicycleColorId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "BicycleSizes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "BicycleSizes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "BicycleColors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WholesalePrices_BicycleSizeId_UserId",
                table: "WholesalePrices",
                columns: new[] { "BicycleSizeId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_BicycleColorId",
                table: "OrderItems",
                column: "BicycleColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_BicycleColors_BicycleColorId",
                table: "OrderItems",
                column: "BicycleColorId",
                principalTable: "BicycleColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_BicycleSizes_BicycleSizeId",
                table: "OrderItems",
                column: "BicycleSizeId",
                principalTable: "BicycleSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WholesalePrices_BicycleSizes_BicycleSizeId",
                table: "WholesalePrices",
                column: "BicycleSizeId",
                principalTable: "BicycleSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
