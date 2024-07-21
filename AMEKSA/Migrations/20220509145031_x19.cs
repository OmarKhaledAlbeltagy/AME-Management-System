using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Accumpained",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "EventTravelRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomType",
                table: "EventTravelRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accumpained",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "HotelName",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "EventTravelRequest");
        }
    }
}
