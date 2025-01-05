using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roomify.Entities.Migrations
{
    /// <inheritdoc />
    public partial class adjustRoomGroupIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "RoomGroups",
                newName: "RoomGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoomGroupId",
                table: "RoomGroups",
                newName: "GroupId");
        }
    }
}
