using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class a8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMedicalVisitChat_AspNetUsers_RepId",
                table: "accountMedicalVisitChat");

            migrationBuilder.DropForeignKey(
                name: "FK_accountSalesVisitChat_AspNetUsers_RepId",
                table: "accountSalesVisitChat");

            migrationBuilder.DropForeignKey(
                name: "FK_contactMedicalVisitChat_AspNetUsers_RepId",
                table: "contactMedicalVisitChat");

            migrationBuilder.AlterColumn<string>(
                name: "RepId",
                table: "contactMedicalVisitChat",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "RepId",
                table: "accountSalesVisitChat",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "RepId",
                table: "accountMedicalVisitChat",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "timeOffTerrirtoryReasons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeOffTerrirtoryReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workingDays",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    NumberOfWorkingDays = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workingDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userTimeOff",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTimeFrom = table.Column<DateTime>(nullable: false),
                    DateTimeTo = table.Column<DateTime>(nullable: false),
                    TimeOffTerritoryReasonsId = table.Column<int>(nullable: false),
                    ExtendIdentityUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userTimeOff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userTimeOff_AspNetUsers_ExtendIdentityUserId",
                        column: x => x.ExtendIdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_userTimeOff_timeOffTerrirtoryReasons_TimeOffTerritoryReasonsId",
                        column: x => x.TimeOffTerritoryReasonsId,
                        principalTable: "timeOffTerrirtoryReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userTimeOff_ExtendIdentityUserId",
                table: "userTimeOff",
                column: "ExtendIdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_userTimeOff_TimeOffTerritoryReasonsId",
                table: "userTimeOff",
                column: "TimeOffTerritoryReasonsId");

            migrationBuilder.AddForeignKey(
                name: "FK_accountMedicalVisitChat_AspNetUsers_RepId",
                table: "accountMedicalVisitChat",
                column: "RepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_accountSalesVisitChat_AspNetUsers_RepId",
                table: "accountSalesVisitChat",
                column: "RepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contactMedicalVisitChat_AspNetUsers_RepId",
                table: "contactMedicalVisitChat",
                column: "RepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accountMedicalVisitChat_AspNetUsers_RepId",
                table: "accountMedicalVisitChat");

            migrationBuilder.DropForeignKey(
                name: "FK_accountSalesVisitChat_AspNetUsers_RepId",
                table: "accountSalesVisitChat");

            migrationBuilder.DropForeignKey(
                name: "FK_contactMedicalVisitChat_AspNetUsers_RepId",
                table: "contactMedicalVisitChat");

            migrationBuilder.DropTable(
                name: "userTimeOff");

            migrationBuilder.DropTable(
                name: "workingDays");

            migrationBuilder.DropTable(
                name: "timeOffTerrirtoryReasons");

            migrationBuilder.AlterColumn<string>(
                name: "RepId",
                table: "contactMedicalVisitChat",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RepId",
                table: "accountSalesVisitChat",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RepId",
                table: "accountMedicalVisitChat",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_accountMedicalVisitChat_AspNetUsers_RepId",
                table: "accountMedicalVisitChat",
                column: "RepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_accountSalesVisitChat_AspNetUsers_RepId",
                table: "accountSalesVisitChat",
                column: "RepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contactMedicalVisitChat_AspNetUsers_RepId",
                table: "contactMedicalVisitChat",
                column: "RepId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
