using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberClub.Migrations
{
    /// <inheritdoc />
    public partial class Changingprofiletypeonmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Images",
                newName: "Url");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "Images",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicUrl",
                table: "BarberShops",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ProfilePicUrl",
                table: "BarberShops");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Images",
                newName: "ImagePath");
        }
    }
}
