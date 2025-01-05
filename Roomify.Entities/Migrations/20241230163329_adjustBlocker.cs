using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomify.Entities.Migrations
{
    /// <inheritdoc />
    public partial class adjustBlocker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Blockers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Blockers",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Blockers");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Blockers");
        }
    }
}
