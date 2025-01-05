using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomify.Entities.Migrations
{
    /// <inheritdoc />
    public partial class addBookingSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "Schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_BookingId",
                table: "Schedules",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Bookings_BookingId",
                table: "Schedules",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Bookings_BookingId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_BookingId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Schedules");
        }
    }
}
