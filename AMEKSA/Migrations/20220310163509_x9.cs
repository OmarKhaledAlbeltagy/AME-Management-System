using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "requestChangeContactTargets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactId = table.Column<int>(nullable: false),
                    OldTarget = table.Column<int>(nullable: false),
                    NewTarget = table.Column<int>(nullable: false),
                    ManagerId = table.Column<string>(nullable: true),
                    RepId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestChangeContactTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestChangeContactTargets_contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requestChangeContactTargets_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_requestChangeContactTargets_AspNetUsers_RepId",
                        column: x => x.RepId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requestChangeContactTargets_ContactId",
                table: "requestChangeContactTargets",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_requestChangeContactTargets_ManagerId",
                table: "requestChangeContactTargets",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_requestChangeContactTargets_RepId",
                table: "requestChangeContactTargets",
                column: "RepId");

            migrationBuilder.AddForeignKey(
                name: "FK_requestChangeCategory_AspNetUsers_ExtendIdentityUserId",
                table: "requestChangeCategory",
                column: "ExtendIdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requestChangeCategory_AspNetUsers_ExtendIdentityUserId",
                table: "requestChangeCategory");

            migrationBuilder.DropTable(
                name: "requestChangeContactTargets");

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
    }
}
