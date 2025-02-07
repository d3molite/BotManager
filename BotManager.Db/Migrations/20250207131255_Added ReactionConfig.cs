using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddedReactionConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReactionConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GuildConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReactionConfigs_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReactionItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    ReactionTrigger = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ReactionMessage = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ReactionEmoji = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    ReactionChance = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReactionItems_ReactionConfigs_ReactionConfigId",
                        column: x => x.ReactionConfigId,
                        principalTable: "ReactionConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReactionConfigs_GuildConfigId",
                table: "ReactionConfigs",
                column: "GuildConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReactionItems_ReactionConfigId",
                table: "ReactionItems",
                column: "ReactionConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReactionItems");

            migrationBuilder.DropTable(
                name: "ReactionConfigs");
        }
    }
}
