using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberClub.Migrations
{
    /// <inheritdoc />
    public partial class Removingbarbershopfromratingsmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_BarberShops_BarberShopId",
                table: "Ratings");

            migrationBuilder.AlterColumn<int>(
                name: "BarberShopId",
                table: "Ratings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_BarberShops_BarberShopId",
                table: "Ratings",
                column: "BarberShopId",
                principalTable: "BarberShops",
                principalColumn: "BarberShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_BarberShops_BarberShopId",
                table: "Ratings");

            migrationBuilder.AlterColumn<int>(
                name: "BarberShopId",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_BarberShops_BarberShopId",
                table: "Ratings",
                column: "BarberShopId",
                principalTable: "BarberShops",
                principalColumn: "BarberShopId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
