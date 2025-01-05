using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomify.Entities.Migrations
{
    /// <inheritdoc />
    public partial class adjustRoomIsAvailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Rooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Rooms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
