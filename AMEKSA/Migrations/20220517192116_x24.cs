using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TopActionUserId",
                table: "EventTravelRequest",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_TopActionUserId",
                table: "EventTravelRequest",
                column: "TopActionUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventTravelRequest_AspNetUsers_TopActionUserId",
                table: "EventTravelRequest",
                column: "TopActionUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventTravelRequest_AspNetUsers_TopActionUserId",
                table: "EventTravelRequest");

            migrationBuilder.DropIndex(
                name: "IX_EventTravelRequest_TopActionUserId",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "TopActionUserId",
                table: "EventTravelRequest");
        }
    }
}
