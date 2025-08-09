using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberClub.Migrations
{
    /// <inheritdoc />
    public partial class Addingprofilepictousermodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Services_ServiceId",
                table: "Ratings");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Services_ServiceId",
                table: "Ratings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Services_ServiceId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "ProfilePicUrl",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Services_ServiceId",
                table: "Ratings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
