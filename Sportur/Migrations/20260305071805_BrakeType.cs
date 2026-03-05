using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class BrakeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrakeType",
                table: "BicycleModels",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrakeType",
                table: "BicycleModels");
        }
    }
}
