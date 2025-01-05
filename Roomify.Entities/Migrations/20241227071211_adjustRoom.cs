using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomify.Entities.Migrations
{
    /// <inheritdoc />
    public partial class adjustRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupGroupId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "RoomGroupGroupId",
                table: "Rooms",
                newName: "RoomGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_RoomGroupGroupId",
                table: "Rooms",
                newName: "IX_Rooms_RoomGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupId",
                table: "Rooms",
                column: "RoomGroupId",
                principalTable: "RoomGroups",
                principalColumn: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "RoomGroupId",
                table: "Rooms",
                newName: "RoomGroupGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_RoomGroupId",
                table: "Rooms",
                newName: "IX_Rooms_RoomGroupGroupId");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupGroupId",
                table: "Rooms",
                column: "RoomGroupGroupId",
                principalTable: "RoomGroups",
                principalColumn: "GroupId");
        }
    }
}
