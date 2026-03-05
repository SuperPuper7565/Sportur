using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class ModelPriceOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceOverride",
                table: "BicycleVariants");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "BicycleModels",
                newName: "Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "BicycleModels",
                newName: "BasePrice");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOverride",
                table: "BicycleVariants",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
