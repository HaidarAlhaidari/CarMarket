using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMarket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCarImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl2",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl3",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl4",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl2",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "ImageUrl3",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "ImageUrl4",
                table: "Cars");
        }
    }
}
