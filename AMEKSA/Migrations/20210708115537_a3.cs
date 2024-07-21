using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductShare",
                table: "accountMedicalVisitProducts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ProductShare",
                table: "accountMedicalVisitProducts",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
