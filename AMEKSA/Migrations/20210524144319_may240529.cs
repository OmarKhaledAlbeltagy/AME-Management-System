using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class may240529 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "openningRequest",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        AccountBrandPaymentId = table.Column<int>(nullable: false),
            //        RequestedOpenning = table.Column<decimal>(type: "money", nullable: false),
            //        ExtendIdentityUserId = table.Column<string>(nullable: true),
            //        Confirmed = table.Column<bool>(nullable: false),
            //        Rejected = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_openningRequest", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_openningRequest_accountBrandPayment_AccountBrandPaymentId",
            //            column: x => x.AccountBrandPaymentId,
            //            principalTable: "accountBrandPayment",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_openningRequest_AspNetUsers_ExtendIdentityUserId",
            //            column: x => x.ExtendIdentityUserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_openningRequest_AccountBrandPaymentId",
            //    table: "openningRequest",
            //    column: "AccountBrandPaymentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_openningRequest_ExtendIdentityUserId",
            //    table: "openningRequest",
            //    column: "ExtendIdentityUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "openningRequest");
        }
    }
}
