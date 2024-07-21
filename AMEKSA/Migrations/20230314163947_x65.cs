using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x65 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_extendidentityuserid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_extendidentityuserid",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "extendidentityuserid",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RepId",
                table: "AspNetUsers",
                column: "RepId",
                unique: true,
                filter: "[RepId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_RepId",
                table: "AspNetUsers",
                column: "RepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_RepId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RepId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RepId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "extendidentityuserid",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_extendidentityuserid",
                table: "AspNetUsers",
                column: "extendidentityuserid");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_extendidentityuserid",
                table: "AspNetUsers",
                column: "extendidentityuserid",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
