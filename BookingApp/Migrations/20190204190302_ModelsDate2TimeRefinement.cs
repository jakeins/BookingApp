using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingApp.Migrations
{
    public partial class ModelsDate2TimeRefinement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "TreeGroups",
                newName: "UpdatedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "TreeGroups",
                newName: "CreatedTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Rules",
                newName: "UpdatedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Rules",
                newName: "CreatedTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Resources",
                newName: "UpdatedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Resources",
                newName: "CreatedTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Bookings",
                newName: "UpdatedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Bookings",
                newName: "CreatedTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "TreeGroups",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "TreeGroups",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "Rules",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "Rules",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "Resources",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "Resources",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "Bookings",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "Bookings",
                newName: "CreatedDate");
        }
    }
}
