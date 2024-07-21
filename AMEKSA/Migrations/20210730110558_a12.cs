using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VacancyDateTime",
                table: "vacancyRequests",
                newName: "VacancyDateTimeTo");

            migrationBuilder.AddColumn<DateTime>(
                name: "VacancyDateTimeFrom",
                table: "vacancyRequests",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VacancyDateTimeFrom",
                table: "vacancyRequests");

            migrationBuilder.RenameColumn(
                name: "VacancyDateTimeTo",
                table: "vacancyRequests",
                newName: "VacancyDateTime");
        }
    }
}
