using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMEKSA.Migrations
{
    /// <inheritdoc />
    public partial class x76 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstActionDateTime",
                table: "EventTravelRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TopActionDateTime",
                table: "EventTravelRequest",
                type: "datetime2",
                nullable: true);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.DropColumn(
                name: "FirstActionDateTime",
                table: "EventTravelRequest");

            migrationBuilder.DropColumn(
                name: "TopActionDateTime",
                table: "EventTravelRequest");

    
        }
    }
}
