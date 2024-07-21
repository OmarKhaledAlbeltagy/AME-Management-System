using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VisitDateTime",
                table: "AccountSupportiveVisit",
                newName: "VisitTime");

            migrationBuilder.RenameColumn(
                name: "SubmittingDateTime",
                table: "AccountSupportiveVisit",
                newName: "VisitDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittingDate",
                table: "AccountSupportiveVisit",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittingTime",
                table: "AccountSupportiveVisit",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittingDate",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropColumn(
                name: "SubmittingTime",
                table: "AccountSupportiveVisit");

            migrationBuilder.RenameColumn(
                name: "VisitTime",
                table: "AccountSupportiveVisit",
                newName: "VisitDateTime");

            migrationBuilder.RenameColumn(
                name: "VisitDate",
                table: "AccountSupportiveVisit",
                newName: "SubmittingDateTime");
        }
    }
}
