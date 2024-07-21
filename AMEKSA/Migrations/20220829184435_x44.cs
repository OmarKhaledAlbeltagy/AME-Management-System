using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x44 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_openningRequest_accountBrandPayment_AccountBrandPaymentId",
                table: "openningRequest");

            migrationBuilder.DropIndex(
                name: "IX_openningRequest_AccountBrandPaymentId",
                table: "openningRequest");

            migrationBuilder.AddColumn<int>(
                name: "openningrequestId",
                table: "openningRequest",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactMedicalVisitId",
                table: "contactMedicalVisit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "contactMedicalVisit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "contactMedicalVisit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_openningRequest_openningrequestId",
                table: "openningRequest",
                column: "openningrequestId");

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit",
                column: "ContactMedicalVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisit_ManagerId1",
                table: "contactMedicalVisit",
                column: "ManagerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_contactMedicalVisit_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit",
                column: "ContactMedicalVisitId",
                principalTable: "contactMedicalVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contactMedicalVisit_AspNetUsers_ManagerId1",
                table: "contactMedicalVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_openningRequest_openningRequest_openningrequestId",
                table: "openningRequest",
                column: "openningrequestId",
                principalTable: "openningRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contactMedicalVisit_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_contactMedicalVisit_AspNetUsers_ManagerId1",
                table: "contactMedicalVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_openningRequest_openningRequest_openningrequestId",
                table: "openningRequest");

            migrationBuilder.DropIndex(
                name: "IX_openningRequest_openningrequestId",
                table: "openningRequest");

            migrationBuilder.DropIndex(
                name: "IX_contactMedicalVisit_ContactMedicalVisitId",
                table: "contactMedicalVisit");

            migrationBuilder.DropIndex(
                name: "IX_contactMedicalVisit_ManagerId1",
                table: "contactMedicalVisit");

            migrationBuilder.DropColumn(
                name: "openningrequestId",
                table: "openningRequest");

            migrationBuilder.DropColumn(
                name: "ContactMedicalVisitId",
                table: "contactMedicalVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "contactMedicalVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "contactMedicalVisit");

            migrationBuilder.CreateIndex(
                name: "IX_openningRequest_AccountBrandPaymentId",
                table: "openningRequest",
                column: "AccountBrandPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_openningRequest_accountBrandPayment_AccountBrandPaymentId",
                table: "openningRequest",
                column: "AccountBrandPaymentId",
                principalTable: "accountBrandPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
