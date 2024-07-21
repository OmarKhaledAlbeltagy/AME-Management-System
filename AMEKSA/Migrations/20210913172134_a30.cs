using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "requestChangeCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExtendIdentityUserId = table.Column<string>(nullable: true),
                    ContactId = table.Column<int>(nullable: false),
                    CategoryFromId = table.Column<int>(nullable: false),
                    CategoryToId = table.Column<int>(nullable: false),
                    Confirmed = table.Column<bool>(nullable: false),
                    Rejected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requestChangeCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_requestChangeCategory_AspNetUsers_ExtendIdentityUserId",
                        column: x => x.ExtendIdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requestChangeCategory_ExtendIdentityUserId",
                table: "requestChangeCategory",
                column: "ExtendIdentityUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "requestChangeCategory");
        }
    }
}
