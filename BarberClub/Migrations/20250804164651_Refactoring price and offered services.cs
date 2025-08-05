using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberClub.Migrations
{
    /// <inheritdoc />
    public partial class Refactoringpriceandofferedservices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferedServices",
                table: "BarberShops");

            migrationBuilder.AlterColumn<string>(
                name: "WorkingDays",
                table: "BarberShops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "OfferedServices",
                columns: table => new
                {
                    OfferedServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BarberShopId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferedServices", x => x.OfferedServiceId);
                    table.ForeignKey(
                        name: "FK_OfferedServices_BarberShops_BarberShopId",
                        column: x => x.BarberShopId,
                        principalTable: "BarberShops",
                        principalColumn: "BarberShopId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferedServices_BarberShopId",
                table: "OfferedServices",
                column: "BarberShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferedServices");

            migrationBuilder.AlterColumn<string>(
                name: "WorkingDays",
                table: "BarberShops",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "OfferedServices",
                table: "BarberShops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
