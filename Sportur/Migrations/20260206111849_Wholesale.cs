using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportur.Migrations
{
    /// <inheritdoc />
    public partial class Wholesale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWholesaleApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWholesaleApproved",
                table: "Users");
        }
    }
}
