using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x53 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "contact",
                newName: "Guidd");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ted",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "ted");

            migrationBuilder.RenameColumn(
                name: "Guidd",
                table: "contact",
                newName: "Guid");
        }
    }
}
