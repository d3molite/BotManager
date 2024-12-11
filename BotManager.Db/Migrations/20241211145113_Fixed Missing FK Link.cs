using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixedMissingFKLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuildConfigs_VoiceChannelConfig_VoiceChannelConfigId",
                table: "GuildConfigs");

            migrationBuilder.DropIndex(
                name: "IX_GuildConfigs_VoiceChannelConfigId",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "VoiceChannelConfigId",
                table: "GuildConfigs");

            migrationBuilder.AddColumn<string>(
                name: "GuildConfigId",
                table: "VoiceChannelConfig",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChannelConfig_GuildConfigId",
                table: "VoiceChannelConfig",
                column: "GuildConfigId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceChannelConfig_GuildConfigs_GuildConfigId",
                table: "VoiceChannelConfig",
                column: "GuildConfigId",
                principalTable: "GuildConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoiceChannelConfig_GuildConfigs_GuildConfigId",
                table: "VoiceChannelConfig");

            migrationBuilder.DropIndex(
                name: "IX_VoiceChannelConfig_GuildConfigId",
                table: "VoiceChannelConfig");

            migrationBuilder.DropColumn(
                name: "GuildConfigId",
                table: "VoiceChannelConfig");

            migrationBuilder.AddColumn<string>(
                name: "VoiceChannelConfigId",
                table: "GuildConfigs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_VoiceChannelConfigId",
                table: "GuildConfigs",
                column: "VoiceChannelConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildConfigs_VoiceChannelConfig_VoiceChannelConfigId",
                table: "GuildConfigs",
                column: "VoiceChannelConfigId",
                principalTable: "VoiceChannelConfig",
                principalColumn: "Id");
        }
    }
}
