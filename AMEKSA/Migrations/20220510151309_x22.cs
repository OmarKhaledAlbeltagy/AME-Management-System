using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_EventId",
                table: "EventTravelRequest",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_Event_EventId",
                table: "EventTravelRequest",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_Event_EventId",
                table: "EventTravelRequest");

            migrationBuilder.DropIndex(
                name: "IX_EventTravelRequest_EventId",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EventTravelRequest");
        }
    }
}
