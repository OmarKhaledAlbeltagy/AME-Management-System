using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x28 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestDeleteAccountSupportive",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountSupportiveVisitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDeleteAccountSupportive", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestDeleteAccountSupportive_AccountSupportiveVisit_AccountSupportiveVisitId",
                        column: x => x.AccountSupportiveVisitId,
                        principalTable: "AccountSupportiveVisit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestDeleteAccountSupportive_AccountSupportiveVisitId",
                table: "RequestDeleteAccountSupportive",
                column: "AccountSupportiveVisitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestDeleteAccountSupportive");
        }
    }
}
