using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class newwwwww : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMedicalVisitProducts_accountMedicalVisit_accountmedicalvisitId",
                table: "accountMedicalVisitProducts");

            migrationBuilder.RenameColumn(
                name: "accountmedicalvisitId",
                table: "accountMedicalVisitProducts",
                newName: "AccountMedicalVisitId");

            migrationBuilder.RenameIndex(
                name: "IX_accountMedicalVisitProducts_accountmedicalvisitId",
                table: "accountMedicalVisitProducts",
                newName: "IX_accountMedicalVisitProducts_AccountMedicalVisitId");

            migrationBuilder.AlterColumn<int>(
                name: "AccountMedicalVisitId",
                table: "accountMedicalVisitProducts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_accountMedicalVisitProducts_accountMedicalVisit_AccountMedicalVisitId",
                table: "accountMedicalVisitProducts",
                column: "AccountMedicalVisitId",
                principalTable: "accountMedicalVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMedicalVisitProducts_accountMedicalVisit_AccountMedicalVisitId",
                table: "accountMedicalVisitProducts");

            migrationBuilder.RenameColumn(
                name: "AccountMedicalVisitId",
                table: "accountMedicalVisitProducts",
                newName: "accountmedicalvisitId");

            migrationBuilder.RenameIndex(
                name: "IX_accountMedicalVisitProducts_AccountMedicalVisitId",
                table: "accountMedicalVisitProducts",
                newName: "IX_accountMedicalVisitProducts_accountmedicalvisitId");

            migrationBuilder.AlterColumn<int>(
                name: "accountmedicalvisitId",
                table: "accountMedicalVisitProducts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_accountMedicalVisitProducts_accountMedicalVisit_accountmedicalvisitId",
                table: "accountMedicalVisitProducts",
                column: "accountmedicalvisitId",
                principalTable: "accountMedicalVisit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
