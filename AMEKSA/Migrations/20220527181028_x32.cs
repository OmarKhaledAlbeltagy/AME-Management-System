using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x32 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountSupportiveVisitChat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagerId = table.Column<string>(nullable: false),
                    RepId = table.Column<string>(nullable: true),
                    ManagerComment = table.Column<string>(maxLength: 500, nullable: false),
                    ManagerCommentDateTime = table.Column<DateTime>(nullable: false),
                    RepReply = table.Column<string>(maxLength: 500, nullable: true),
                    RepReplyDateTime = table.Column<DateTime>(nullable: false),
                    AccountSupportiveVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSupportiveVisitChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitChat_AccountSupportiveVisit_AccountSupportiveVisitId",
                        column: x => x.AccountSupportiveVisitId,
                        principalTable: "AccountSupportiveVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitChat_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountSupportiveVisitChat_AspNetUsers_RepId",
                        column: x => x.RepId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitChat_AccountSupportiveVisitId",
                table: "AccountSupportiveVisitChat",
                column: "AccountSupportiveVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitChat_ManagerId",
                table: "AccountSupportiveVisitChat",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSupportiveVisitChat_RepId",
                table: "AccountSupportiveVisitChat",
                column: "RepId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSupportiveVisitChat");
        }
    }
}
