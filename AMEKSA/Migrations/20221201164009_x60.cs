using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x60 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTicket",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TicketFileContentType",
                table: "EventTravelRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketFileName",
                table: "EventTravelRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTicket",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "TicketFileContentType",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "TicketFileName",
                table: "EventTravelRequest");
        }
    }
}
