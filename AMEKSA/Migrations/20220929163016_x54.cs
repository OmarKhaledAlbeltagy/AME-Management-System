using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x54 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ted",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "ted",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Registered",
                table: "ted",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ted");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "ted");

            migrationBuilder.DropColumn(
                name: "Registered",
                table: "ted");
        }
    }
}
