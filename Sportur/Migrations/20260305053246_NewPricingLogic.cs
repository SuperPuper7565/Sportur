using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class NewPricingLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WholesalePrices");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "BicycleModels",
                newName: "WholesalePrice");

            migrationBuilder.AddColumn<decimal>(
                name: "RetailPrice",
                table: "BicycleModels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetailPrice",
                table: "BicycleModels");

            migrationBuilder.RenameColumn(
                name: "WholesalePrice",
                table: "BicycleModels",
                newName: "Price");

            migrationBuilder.CreateTable(
                name: "WholesalePrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BicycleVariantId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WholesalePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WholesalePrices_BicycleVariants_BicycleVariantId",
                        column: x => x.BicycleVariantId,
                        principalTable: "BicycleVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WholesalePrices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WholesalePrices_BicycleVariantId_UserId",
                table: "WholesalePrices",
                columns: new[] { "BicycleVariantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WholesalePrices_UserId",
                table: "WholesalePrices",
                column: "UserId");
        }
    }
}
