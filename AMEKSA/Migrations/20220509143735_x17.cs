using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TravelCitiesId",
                table: "Event",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Event_TravelCitiesId",
                table: "Event",
                column: "TravelCitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_travelCities_TravelCitiesId",
                table: "Event",
                column: "TravelCitiesId",
                principalTable: "travelCities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_travelCities_TravelCitiesId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_TravelCitiesId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "TravelCitiesId",
                table: "Event");
        }
    }
}
