using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x45 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contactMedicalVisit_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit");

            migrationBuilder.DropIndex(
                name: "IX_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit");

            migrationBuilder.DropColumn(
                name: "ContactMedicalVisitId",
                table: "contactMedicalVisit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactMedicalVisitId",
                table: "contactMedicalVisit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit",
                column: "ContactMedicalVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_contactMedicalVisit_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit",
                column: "ContactMedicalVisitId",
                principalTable: "contactMedicalVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
