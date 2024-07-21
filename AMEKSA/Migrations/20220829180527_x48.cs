using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x48 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMedicalVisit_AspNetUsers_ManagerId1",
                table: "accountMedicalVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_accountSalesVisit_AspNetUsers_ManagerId1",
                table: "accountSalesVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountSupportiveVisit_AspNetUsers_ManagerId1",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_contactMedicalVisit_AspNetUsers_ManagerId1",
                table: "contactMedicalVisit");

            migrationBuilder.DropIndex(
                name: "IX_contactMedicalVisit_ManagerId1",
                table: "contactMedicalVisit");

            migrationBuilder.DropIndex(
                name: "IX_AccountSupportiveVisit_ManagerId1",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropIndex(
                name: "IX_accountSalesVisit_ManagerId1",
                table: "accountSalesVisit");

            migrationBuilder.DropIndex(
                name: "IX_accountMedicalVisit_ManagerId1",
                table: "accountMedicalVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "contactMedicalVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "accountSalesVisit");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "accountMedicalVisit");

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "contactMedicalVisit",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "AccountSupportiveVisit",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "accountSalesVisit",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "accountMedicalVisit",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisit_ManagerId",
                table: "contactMedicalVisit",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisit_ManagerId",
                table: "AccountSupportiveVisit",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisit_ManagerId",
                table: "accountSalesVisit",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_accountMedicalVisit_ManagerId",
                table: "accountMedicalVisit",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMedicalVisit_AspNetUsers_ManagerId",
                table: "accountMedicalVisit",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_accountSalesVisit_AspNetUsers_ManagerId",
                table: "accountSalesVisit",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountSupportiveVisit_AspNetUsers_ManagerId",
                table: "AccountSupportiveVisit",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contactMedicalVisit_AspNetUsers_ManagerId",
                table: "contactMedicalVisit",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMedicalVisit_AspNetUsers_ManagerId",
                table: "accountMedicalVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_accountSalesVisit_AspNetUsers_ManagerId",
                table: "accountSalesVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountSupportiveVisit_AspNetUsers_ManagerId",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropForeignKey(
                name: "FK_contactMedicalVisit_AspNetUsers_ManagerId",
                table: "contactMedicalVisit");

            migrationBuilder.DropIndex(
                name: "IX_contactMedicalVisit_ManagerId",
                table: "contactMedicalVisit");

            migrationBuilder.DropIndex(
                name: "IX_AccountSupportiveVisit_ManagerId",
                table: "AccountSupportiveVisit");

            migrationBuilder.DropIndex(
                name: "IX_accountSalesVisit_ManagerId",
                table: "accountSalesVisit");

            migrationBuilder.DropIndex(
                name: "IX_accountMedicalVisit_ManagerId",
                table: "accountMedicalVisit");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "contactMedicalVisit",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "contactMedicalVisit",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "AccountSupportiveVisit",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "AccountSupportiveVisit",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "accountSalesVisit",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "accountSalesVisit",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "accountMedicalVisit",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "accountMedicalVisit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contactMedicalVisit_ManagerId1",
                table: "contactMedicalVisit",
                column: "ManagerId1");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisit_ManagerId1",
                table: "AccountSupportiveVisit",
                column: "ManagerId1");

            migrationBuilder.CreateIndex(
                name: "IX_accountSalesVisit_ManagerId1",
                table: "accountSalesVisit",
                column: "ManagerId1");

            migrationBuilder.CreateIndex(
                name: "IX_accountMedicalVisit_ManagerId1",
                table: "accountMedicalVisit",
                column: "ManagerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMedicalVisit_AspNetUsers_ManagerId1",
                table: "accountMedicalVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_accountSalesVisit_AspNetUsers_ManagerId1",
                table: "accountSalesVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountSupportiveVisit_AspNetUsers_ManagerId1",
                table: "AccountSupportiveVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contactMedicalVisit_AspNetUsers_ManagerId1",
                table: "contactMedicalVisit",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
