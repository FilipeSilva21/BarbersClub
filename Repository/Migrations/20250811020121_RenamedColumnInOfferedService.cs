using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberClub.Migrations
{
    /// <inheritdoc />
    public partial class RenamedColumnInOfferedService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceTypeType",
                table: "OfferedServices",
                newName: "ServiceType");

            migrationBuilder.RenameIndex(
                name: "IX_OfferedServices_BarberShopId_ServiceTypeType",
                table: "OfferedServices",
                newName: "IX_OfferedServices_BarberShopId_ServiceType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceType",
                table: "OfferedServices",
                newName: "ServiceTypeType");

            migrationBuilder.RenameIndex(
                name: "IX_OfferedServices_BarberShopId_ServiceType",
                table: "OfferedServices",
                newName: "IX_OfferedServices_BarberShopId_ServiceTypeType");
        }
    }
}
