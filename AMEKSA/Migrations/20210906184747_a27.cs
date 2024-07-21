using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBalance_account_AccountId",
                table: "AccountBalance");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountBalance_brand_BrandId",
                table: "AccountBalance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountBalance",
                table: "AccountBalance");

            migrationBuilder.RenameTable(
                name: "AccountBalance",
                newName: "accountBalance");

            migrationBuilder.RenameIndex(
                name: "IX_AccountBalance_BrandId",
                table: "accountBalance",
                newName: "IX_accountBalance_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountBalance_AccountId",
                table: "accountBalance",
                newName: "IX_accountBalance_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accountBalance",
                table: "accountBalance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_accountBalance_account_AccountId",
                table: "accountBalance",
                column: "AccountId",
                principalTable: "account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_accountBalance_brand_BrandId",
                table: "accountBalance",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountBalance_account_AccountId",
                table: "accountBalance");

            migrationBuilder.DropForeignKey(
                name: "FK_accountBalance_brand_BrandId",
                table: "accountBalance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accountBalance",
                table: "accountBalance");

            migrationBuilder.RenameTable(
                name: "accountBalance",
                newName: "AccountBalance");

            migrationBuilder.RenameIndex(
                name: "IX_accountBalance_BrandId",
                table: "AccountBalance",
                newName: "IX_AccountBalance_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_accountBalance_AccountId",
                table: "AccountBalance",
                newName: "IX_AccountBalance_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountBalance",
                table: "AccountBalance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBalance_account_AccountId",
                table: "AccountBalance",
                column: "AccountId",
                principalTable: "account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBalance_brand_BrandId",
                table: "AccountBalance",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
