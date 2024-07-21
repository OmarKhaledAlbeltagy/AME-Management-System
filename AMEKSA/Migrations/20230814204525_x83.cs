using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMEKSA.Migrations
{
    /// <inheritdoc />
    public partial class x83 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangingDateTime",
                table: "passwordChange",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangingDateTime",
                table: "passwordChange");
        }
    }
}
