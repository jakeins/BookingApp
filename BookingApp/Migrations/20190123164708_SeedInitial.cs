using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace BookingApp.Migrations
{
    public partial class SeedInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Feeding raw SQL from a file; gets copied into output folder by File Properties.
            var seedingSqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations\20190123164708_SeedInitial.cs.sql");
            migrationBuilder.Sql(File.ReadAllText(seedingSqlPath));
        }

        // Caution! Reverting this migration clears the corresponding DB tables completely.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [dbo].[AspNetRoles]
                DELETE FROM [dbo].[AspNetUsers]
                DELETE FROM [dbo].[AspNetUserRoles]
                DELETE FROM [dbo].[Rules]
                DELETE FROM [dbo].[TreeGroups]
                DELETE FROM [dbo].[Resources]
                DELETE FROM [dbo].[Bookings]
                ");
        }
    }
}
