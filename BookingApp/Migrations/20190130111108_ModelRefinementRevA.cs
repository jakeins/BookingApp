using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingApp.Migrations
{
    public partial class ModelRefinementRevA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TreeGroups",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Rules",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Resources",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Resources",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "Bookings",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Bookings",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "TreeGroups",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Rules",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Resources",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Resources",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCancelled",
                table: "Bookings",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Bookings",
                nullable: true);
        }
    }
}
