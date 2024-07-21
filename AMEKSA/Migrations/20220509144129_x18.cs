using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventTravelRequest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExtendIdentityUserId = table.Column<string>(nullable: true),
                    ContactId = table.Column<int>(nullable: false),
                    WayInDate = table.Column<DateTime>(nullable: false),
                    WayOutDate = table.Column<DateTime>(nullable: false),
                    WayInCityId = table.Column<int>(nullable: false),
                    WayOutCityId = table.Column<int>(nullable: false),
                    WayInFlightNumber = table.Column<string>(nullable: true),
                    WayOutFlightNumber = table.Column<string>(nullable: true),
                    WayInDestinationId = table.Column<int>(nullable: false),
                    WayOutDestinationId = table.Column<int>(nullable: false),
                    Confirmed = table.Column<bool>(nullable: false),
                    Rejected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTravelRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTravelRequest_contact_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTravelRequest_AspNetUsers_ExtendIdentityUserId",
                        column: x => x.ExtendIdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTravelRequest_travelCities_WayInCityId",
                        column: x => x.WayInCityId,
                        principalTable: "travelCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict,
                        onUpdate: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTravelRequest_travelCities_WayInDestinationId",
                        column: x => x.WayInDestinationId,
                        principalTable: "travelCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict,
                        onUpdate: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTravelRequest_travelCities_WayOutCityId",
                        column: x => x.WayOutCityId,
                        principalTable: "travelCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict,
                        onUpdate: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTravelRequest_travelCities_WayOutDestinationId",
                        column: x => x.WayOutDestinationId,
                        principalTable: "travelCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict,
                        onUpdate: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_ContactId",
                table: "EventTravelRequest",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_ExtendIdentityUserId",
                table: "EventTravelRequest",
                column: "ExtendIdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_WayInCityId",
                table: "EventTravelRequest",
                column: "WayInCityId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_WayInDestinationId",
                table: "EventTravelRequest",
                column: "WayInDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_WayOutCityId",
                table: "EventTravelRequest",
                column: "WayOutCityId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTravelRequest_WayOutDestinationId",
                table: "EventTravelRequest",
                column: "WayOutDestinationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTravelRequest");
        }
    }
}
