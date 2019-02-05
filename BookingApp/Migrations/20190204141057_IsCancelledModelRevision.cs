using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingApp.Migrations
{
    public partial class IsCancelledModelRevision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Bookings");

            migrationBuilder.AddColumn<DateTime>(
                name: "TerminationTime",
                table: "Bookings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TerminationTime",
                table: "Bookings");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Bookings",
                nullable: true,
                defaultValue: false);
        }
    }
}
