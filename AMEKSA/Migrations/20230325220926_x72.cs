using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x72 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "trainingRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "maintenanceRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "trainingRequest");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "maintenanceRequest");
        }
    }
}
