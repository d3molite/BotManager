using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddedModmailConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModMailConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ForumChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    PingRoleId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    BotConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModMailConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModMailConfigs_Configs_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModMailConfigs_BotConfigId",
                table: "ModMailConfigs",
                column: "BotConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModMailConfigs");
        }
    }
}
