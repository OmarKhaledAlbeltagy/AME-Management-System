using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventProposalRequest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(nullable: false),
                    ExtendIdentityUserId = table.Column<string>(nullable: true),
                    ContactId = table.Column<int>(nullable: false),
                    Confirmed = table.Column<bool>(nullable: false),
                    Rejected = table.Column<bool>(nullable: false),
                    TopConfirmed = table.Column<bool>(nullable: false),
                    TopRejected = table.Column<bool>(nullable: false),
                    TopAction = table.Column<bool>(nullable: false),
                    TopActionUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventProposalRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventProposalRequest_contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventProposalRequest_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventProposalRequest_AspNetUsers_ExtendIdentityUserId",
                        column: x => x.ExtendIdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventProposalRequest_AspNetUsers_TopActionUserId",
                        column: x => x.TopActionUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventProposalRequest_ContactId",
                table: "EventProposalRequest",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_EventProposalRequest_EventId",
                table: "EventProposalRequest",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventProposalRequest_ExtendIdentityUserId",
                table: "EventProposalRequest",
                column: "ExtendIdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventProposalRequest_TopActionUserId",
                table: "EventProposalRequest",
                column: "TopActionUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventProposalRequest");
        }
    }
}
