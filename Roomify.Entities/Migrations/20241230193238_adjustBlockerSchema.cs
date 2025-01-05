using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Roomify.Entities.Migrations
{
    /// <inheritdoc />
    public partial class adjustBlockerSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blockers_BlockerRoomChoices_BlockerRoomChoiceId",
                table: "Blockers");

            migrationBuilder.DropTable(
                name: "BlockerRoomChoices");

            migrationBuilder.DropTable(
                name: "RoomAffecteds");

            migrationBuilder.DropIndex(
                name: "IX_Blockers_BlockerRoomChoiceId",
                table: "Blockers");

            migrationBuilder.DropColumn(
                name: "BlockerRoomChoiceId",
                table: "Blockers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockerRoomChoiceId",
                table: "Blockers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BlockerRoomChoices",
                columns: table => new
                {
                    BlockerRoomChoiceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockerRoomChoices", x => x.BlockerRoomChoiceId);
                });

            migrationBuilder.CreateTable(
                name: "RoomAffecteds",
                columns: table => new
                {
                    RoomAffectedId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlockerId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAffecteds", x => x.RoomAffectedId);
                    table.ForeignKey(
                        name: "FK_RoomAffecteds_Blockers_BlockerId",
                        column: x => x.BlockerId,
                        principalTable: "Blockers",
                        principalColumn: "BlockerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomAffecteds_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blockers_BlockerRoomChoiceId",
                table: "Blockers",
                column: "BlockerRoomChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAffecteds_BlockerId",
                table: "RoomAffecteds",
                column: "BlockerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAffecteds_RoomId",
                table: "RoomAffecteds",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blockers_BlockerRoomChoices_BlockerRoomChoiceId",
                table: "Blockers",
                column: "BlockerRoomChoiceId",
                principalTable: "BlockerRoomChoices",
                principalColumn: "BlockerRoomChoiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
