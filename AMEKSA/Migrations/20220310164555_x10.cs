using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "requestChangeContactTargets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "requestChangeContactTargets");
        }
    }
}
