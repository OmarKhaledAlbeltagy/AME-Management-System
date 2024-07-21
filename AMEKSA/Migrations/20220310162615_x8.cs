using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requestChangeCategory_AspNetUsers_ExtendIdentityUserId",
                table: "requestChangeCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_requestChangeCategory",
                table: "requestChangeCategory");

            migrationBuilder.RenameTable(
                name: "requestChangeCategory",
                newName: "RequestChangeContactCategory");

            migrationBuilder.RenameIndex(
                name: "IX_requestChangeCategory_ExtendIdentityUserId",
                table: "RequestChangeContactCategory",
                newName: "IX_RequestChangeContactCategory_ExtendIdentityUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestChangeContactCategory",
                table: "RequestChangeContactCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestChangeContactCategory_AspNetUsers_ExtendIdentityUserId",
                table: "RequestChangeContactCategory",
                column: "ExtendIdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestChangeContactCategory_AspNetUsers_ExtendIdentityUserId",
                table: "RequestChangeContactCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestChangeContactCategory",
                table: "RequestChangeContactCategory");

            migrationBuilder.RenameTable(
                name: "RequestChangeContactCategory",
                newName: "requestChangeCategory");

            migrationBuilder.RenameIndex(
                name: "IX_RequestChangeContactCategory_ExtendIdentityUserId",
                table: "requestChangeCategory",
                newName: "IX_requestChangeCategory_ExtendIdentityUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_requestChangeCategory",
                table: "requestChangeCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_requestChangeCategory_AspNetUsers_ExtendIdentityUserId",
                table: "requestChangeCategory",
                column: "ExtendIdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
