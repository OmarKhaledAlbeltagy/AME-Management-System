using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyTarget",
                table: "contact");

            migrationBuilder.AddColumn<int>(
                name: "MonthlyTarget",
                table: "userContact",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyTarget",
                table: "userContact");

            migrationBuilder.AddColumn<int>(
                name: "MonthlyTarget",
                table: "contact",
                nullable: true);
        }
    }
}
