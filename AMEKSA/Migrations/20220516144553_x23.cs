using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TopAction",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TopConfirmed",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TopRejected",
                table: "EventTravelRequest",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopAction",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "TopConfirmed",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "TopRejected",
                table: "EventTravelRequest");
        }
    }
}
