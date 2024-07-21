using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMEKSA.Migrations
{
    /// <inheritdoc />
    public partial class x80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EngineerName",
                table: "maintenanceRequest",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EngineerName",
                table: "maintenanceRequest");
        }
    }
}
