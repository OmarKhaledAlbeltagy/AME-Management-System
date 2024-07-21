using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMEKSA.Migrations
{
    /// <inheritdoc />
    public partial class x78 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "ClientResponse",
                table: "trainingRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagementResponse",
                table: "trainingRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientResponse",
                table: "maintenanceRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagementResponse",
                table: "maintenanceRequest",
                type: "nvarchar(max)",
                nullable: true);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientResponse",
                table: "trainingRequest");

            migrationBuilder.DropColumn(
                name: "ManagementResponse",
                table: "trainingRequest");

            migrationBuilder.DropColumn(
                name: "ClientResponse",
                table: "maintenanceRequest");

            migrationBuilder.DropColumn(
                name: "ManagementResponse",
                table: "maintenanceRequest");
        }
    }
}
