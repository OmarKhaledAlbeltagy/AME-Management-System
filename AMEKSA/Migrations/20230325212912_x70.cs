using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x70 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maintenanceRequest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountDeviceId = table.Column<int>(nullable: false),
                    accountDevicesId = table.Column<int>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: true),
                    LandLineNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    brief = table.Column<string>(nullable: true),
                    RequestDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenanceRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_maintenanceRequest_accountDevices_accountDevicesId",
                        column: x => x.accountDevicesId,
                        principalTable: "accountDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trainingRequest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountDeviceId = table.Column<int>(nullable: false),
                    accountDevicesId = table.Column<int>(nullable: true),
                    MobileNumber = table.Column<string>(nullable: true),
                    LandLineNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    RequestedDate = table.Column<DateTime>(nullable: false),
                    RequestDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trainingRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trainingRequest_accountDevices_accountDevicesId",
                        column: x => x.accountDevicesId,
                        principalTable: "accountDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_maintenanceRequest_accountDevicesId",
                table: "maintenanceRequest",
                column: "accountDevicesId");

            migrationBuilder.CreateIndex(
                name: "IX_trainingRequest_accountDevicesId",
                table: "trainingRequest",
                column: "accountDevicesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maintenanceRequest");

            migrationBuilder.DropTable(
                name: "trainingRequest");
        }
    }
}
