using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x56 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Registered",
                table: "ted",
                newName: "Sms");

            migrationBuilder.AddColumn<bool>(
                name: "Attend",
                table: "ted",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendTime",
                table: "ted",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attend",
                table: "ted");

            migrationBuilder.DropColumn(
                name: "AttendTime",
                table: "ted");

            migrationBuilder.RenameColumn(
                name: "Sms",
                table: "ted",
                newName: "Registered");
        }
    }
}
