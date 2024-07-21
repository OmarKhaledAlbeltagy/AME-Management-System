using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class _68 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceContract",
                table: "accountDevices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Warranty",
                table: "accountDevices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceContract",
                table: "accountDevices");

            migrationBuilder.DropColumn(
                name: "Warranty",
                table: "accountDevices");
        }
    }
}
