using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberClub.Migrations
{
    /// <inheritdoc />
    public partial class RenamedServiceTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Services",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceType",
                table: "OfferedServices",
                newName: "ServiceTypeType");

            migrationBuilder.RenameIndex(
                name: "IX_OfferedServices_BarberShopId_ServiceType",
                table: "OfferedServices",
                newName: "IX_OfferedServices_BarberShopId_ServiceTypeType");

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceTypeType",
                table: "OfferedServices",
                newName: "ServiceType");

            migrationBuilder.RenameIndex(
                name: "IX_OfferedServices_BarberShopId_ServiceTypeType",
                table: "OfferedServices",
                newName: "IX_OfferedServices_BarberShopId_ServiceType");

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
