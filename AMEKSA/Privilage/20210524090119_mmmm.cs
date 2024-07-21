using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class mmmm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtendIdentityUserId",
                table: "openningRequest",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_openningRequest_ExtendIdentityUserId",
                table: "openningRequest",
                column: "ExtendIdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_openningRequest_AspNetUsers_ExtendIdentityUserId",
                table: "openningRequest",
                column: "ExtendIdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_openningRequest_AspNetUsers_ExtendIdentityUserId",
                table: "openningRequest");

            migrationBuilder.DropIndex(
                name: "IX_openningRequest_ExtendIdentityUserId",
                table: "openningRequest");

            migrationBuilder.DropColumn(
                name: "ExtendIdentityUserId",
                table: "openningRequest");
        }
    }
}
