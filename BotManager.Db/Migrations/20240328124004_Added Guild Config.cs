using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddedGuildConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderTrackingConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTrackingConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    OrderTrackingConfigId = table.Column<string>(type: "TEXT", nullable: true),
                    BotConfigId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildConfigs_Configs_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "Configs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GuildConfigs_OrderTrackingConfigs_OrderTrackingConfigId",
                        column: x => x.OrderTrackingConfigId,
                        principalTable: "OrderTrackingConfigs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_BotConfigId",
                table: "GuildConfigs",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_OrderTrackingConfigId",
                table: "GuildConfigs",
                column: "OrderTrackingConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "OrderTrackingConfigs");
        }
    }
}
