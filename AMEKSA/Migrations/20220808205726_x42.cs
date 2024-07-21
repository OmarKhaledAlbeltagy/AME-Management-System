using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x42 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInCityId",
                table: "EventTravelRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInDestinationId",
                table: "EventTravelRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutCityId",
                table: "EventTravelRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutDestinationId",
                table: "EventTravelRequest");

            migrationBuilder.AlterColumn<int>(
                name: "WayOutDestinationId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayOutDeparture",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "WayOutCityId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayOutArrival",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "WayInDestinationId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayInDeparture",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "WayInCityId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayInArrival",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInCityId",
                table: "EventTravelRequest",
                column: "WayInCityId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInDestinationId",
                table: "EventTravelRequest",
                column: "WayInDestinationId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutCityId",
                table: "EventTravelRequest",
                column: "WayOutCityId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutDestinationId",
                table: "EventTravelRequest",
                column: "WayOutDestinationId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInCityId",
                table: "EventTravelRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInDestinationId",
                table: "EventTravelRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutCityId",
                table: "EventTravelRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutDestinationId",
                table: "EventTravelRequest");

            migrationBuilder.AlterColumn<int>(
                name: "WayOutDestinationId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayOutDeparture",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WayOutCityId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayOutArrival",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WayInDestinationId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayInDeparture",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WayInCityId",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "WayInArrival",
                table: "EventTravelRequest",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInCityId",
                table: "EventTravelRequest",
                column: "WayInCityId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayInDestinationId",
                table: "EventTravelRequest",
                column: "WayInDestinationId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutCityId",
                table: "EventTravelRequest",
                column: "WayOutCityId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_travelCities_WayOutDestinationId",
                table: "EventTravelRequest",
                column: "WayOutDestinationId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
