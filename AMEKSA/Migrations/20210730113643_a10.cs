using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vacancyRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    VacancyDateTime = table.Column<DateTime>(nullable: false),
                    extendidentityuserid = table.Column<string>(nullable: true),
                    Accepted = table.Column<bool>(nullable: false),
                    Rejected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacancyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vacancyRequests_AspNetUsers_extendidentityuserid",
                        column: x => x.extendidentityuserid,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vacancyRequests_extendidentityuserid",
                table: "vacancyRequests",
                column: "extendidentityuserid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vacancyRequests");
        }
    }
}
