using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyTarget",
                table: "contact");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlyTarget",
                table: "contact",
                nullable: true);
        }
    }
}
