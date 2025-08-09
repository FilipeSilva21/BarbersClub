using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberClub.Migrations
{
    /// <inheritdoc />
    public partial class Changingservicepicinservicemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceImageUrl",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceImageUrl",
                table: "Services");
        }
    }
}
