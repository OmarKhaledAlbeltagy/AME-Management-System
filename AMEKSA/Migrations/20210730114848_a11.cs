using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeOffTerritoryReasonsId",
                table: "vacancyRequests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_vacancyRequests_TimeOffTerritoryReasonsId",
                table: "vacancyRequests",
                column: "TimeOffTerritoryReasonsId");

            migrationBuilder.AddForeignKey(
                name: "FK_vacancyRequests_timeOffTerrirtoryReasons_TimeOffTerritoryReasonsId",
                table: "vacancyRequests",
                column: "TimeOffTerritoryReasonsId",
                principalTable: "timeOffTerrirtoryReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vacancyRequests_timeOffTerrirtoryReasons_TimeOffTerritoryReasonsId",
                table: "vacancyRequests");

            migrationBuilder.DropIndex(
                name: "IX_vacancyRequests_TimeOffTerritoryReasonsId",
                table: "vacancyRequests");

            migrationBuilder.DropColumn(
                name: "TimeOffTerritoryReasonsId",
                table: "vacancyRequests");
        }
    }
}
