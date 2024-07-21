using Microsoft.EntityFrameworkCore.Migrations;

namespace AMEKSA.Migrations
{
    public partial class x71 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_maintenanceRequest_accountDevices_accountDevicesId",
                table: "maintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_trainingRequest_accountDevices_accountDevicesId",
                table: "trainingRequest");

            migrationBuilder.DropColumn(
                name: "AccountDeviceId",
                table: "trainingRequest");

            migrationBuilder.DropColumn(
                name: "AccountDeviceId",
                table: "maintenanceRequest");

            migrationBuilder.RenameColumn(
                name: "accountDevicesId",
                table: "trainingRequest",
                newName: "AccountDevicesId");

            migrationBuilder.RenameIndex(
                name: "IX_trainingRequest_accountDevicesId",
                table: "trainingRequest",
                newName: "IX_trainingRequest_AccountDevicesId");

            migrationBuilder.RenameColumn(
                name: "accountDevicesId",
                table: "maintenanceRequest",
                newName: "AccountDevicesId");

            migrationBuilder.RenameIndex(
                name: "IX_maintenanceRequest_accountDevicesId",
                table: "maintenanceRequest",
                newName: "IX_maintenanceRequest_AccountDevicesId");

            migrationBuilder.AlterColumn<int>(
                name: "AccountDevicesId",
                table: "trainingRequest",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountDevicesId",
                table: "maintenanceRequest",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_maintenanceRequest_accountDevices_AccountDevicesId",
                table: "maintenanceRequest",
                column: "AccountDevicesId",
                principalTable: "accountDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trainingRequest_accountDevices_AccountDevicesId",
                table: "trainingRequest",
                column: "AccountDevicesId",
                principalTable: "accountDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_maintenanceRequest_accountDevices_AccountDevicesId",
                table: "maintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_trainingRequest_accountDevices_AccountDevicesId",
                table: "trainingRequest");

            migrationBuilder.RenameColumn(
                name: "AccountDevicesId",
                table: "trainingRequest",
                newName: "accountDevicesId");

            migrationBuilder.RenameIndex(
                name: "IX_trainingRequest_AccountDevicesId",
                table: "trainingRequest",
                newName: "IX_trainingRequest_accountDevicesId");

            migrationBuilder.RenameColumn(
                name: "AccountDevicesId",
                table: "maintenanceRequest",
                newName: "accountDevicesId");

            migrationBuilder.RenameIndex(
                name: "IX_maintenanceRequest_AccountDevicesId",
                table: "maintenanceRequest",
                newName: "IX_maintenanceRequest_accountDevicesId");

            migrationBuilder.AlterColumn<int>(
                name: "accountDevicesId",
                table: "trainingRequest",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AccountDeviceId",
                table: "trainingRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "accountDevicesId",
                table: "maintenanceRequest",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "AccountDeviceId",
                table: "maintenanceRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_maintenanceRequest_accountDevices_accountDevicesId",
                table: "maintenanceRequest",
                column: "accountDevicesId",
                principalTable: "accountDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_trainingRequest_accountDevices_accountDevicesId",
                table: "trainingRequest",
                column: "accountDevicesId",
                principalTable: "accountDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
