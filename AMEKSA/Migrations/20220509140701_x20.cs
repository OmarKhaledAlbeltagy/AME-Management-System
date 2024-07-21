using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WayOutDate",
                table: "EventTravelRequest",
                newName: "WayOutDeparture");

            migrationBuilder.RenameColumn(
                name: "WayInDate",
                table: "EventTravelRequest",
                newName: "WayOutArrival");

            migrationBuilder.AddColumn<DateTime>(
                name: "WayInArrival",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WayInDeparture",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WayInArrival",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "WayInDeparture",
                table: "EventTravelRequest");

            migrationBuilder.RenameColumn(
                name: "WayOutDeparture",
                table: "EventTravelRequest",
                newName: "WayOutDate");

            migrationBuilder.RenameColumn(
                name: "WayOutArrival",
                table: "EventTravelRequest",
                newName: "WayInDate");
        }
    }
}
