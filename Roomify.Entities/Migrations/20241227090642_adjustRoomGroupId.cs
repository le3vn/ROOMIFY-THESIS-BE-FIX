using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomify.Entities.Migrations
{
    /// <inheritdoc />
    public partial class adjustRoomGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "RoomGroupId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupId",
                table: "Rooms",
                column: "RoomGroupId",
                principalTable: "RoomGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "RoomGroupId",
                table: "Rooms",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomGroups_RoomGroupId",
                table: "Rooms",
                column: "RoomGroupId",
                principalTable: "RoomGroups",
                principalColumn: "GroupId");
        }
    }
}
