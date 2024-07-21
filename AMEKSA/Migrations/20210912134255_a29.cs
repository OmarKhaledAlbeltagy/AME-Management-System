using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a29 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "userContact",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "account",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "account",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

      

            migrationBuilder.CreateIndex(
                name: "IX_userContact_CategoryId",
                table: "userContact",
                column: "CategoryId");

  

     

            migrationBuilder.AddForeignKey(
                name: "FK_userContact_category_CategoryId",
                table: "userContact",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userContact_category_CategoryId",
                table: "userContact");

        

            migrationBuilder.DropIndex(
                name: "IX_userContact_CategoryId",
                table: "userContact");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "userContact");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "account",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "account",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
