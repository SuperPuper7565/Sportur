using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class NewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accessories",
                table: "BicycleModels",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BottomBracket",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cassette",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Chain",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Crankset",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrontDerailleur",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Headset",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Hubs",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RearDerailleur",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rims",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Shifters",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tires",
                table: "BicycleModels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accessories",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "BottomBracket",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Cassette",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Chain",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Crankset",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "FrontDerailleur",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Headset",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Hubs",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "RearDerailleur",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Rims",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Shifters",
                table: "BicycleModels");

            migrationBuilder.DropColumn(
                name: "Tires",
                table: "BicycleModels");
        }
    }
}
