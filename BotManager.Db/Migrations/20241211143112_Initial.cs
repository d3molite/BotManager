using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotManager.Db.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Token = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Presence = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    Debug = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    IsOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDelivered = table.Column<bool>(type: "INTEGER", nullable: false),
                    RestaurantName = table.Column<string>(type: "TEXT", nullable: false),
                    OrderTime = table.Column<string>(type: "TEXT", nullable: false),
                    MenuLink = table.Column<string>(type: "TEXT", nullable: false),
                    PaypalLink = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoiceChannelConfig",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CommandChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    VoiceCategoryId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceChannelConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ItemAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    VoiceChannelConfigId = table.Column<string>(type: "TEXT", nullable: true),
                    BotConfigId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuildConfigs_Configs_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "Configs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildConfigs_VoiceChannelConfig_VoiceChannelConfigId",
                        column: x => x.VoiceChannelConfigId,
                        principalTable: "VoiceChannelConfig",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BirthdayConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    PingChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthdayConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthdayConfigs_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GuildConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageConfigs_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoggingConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    LoggingChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoggingConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoggingConfigs_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTrackingConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    GuildConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTrackingConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTrackingConfigs_GuildConfigs_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "GuildConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Birthdays",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    BirthdayConfigId = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Birthdays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Birthdays_BirthdayConfigs_BirthdayConfigId",
                        column: x => x.BirthdayConfigId,
                        principalTable: "BirthdayConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BirthdayConfigs_GuildConfigId",
                table: "BirthdayConfigs",
                column: "GuildConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Birthdays_BirthdayConfigId",
                table: "Birthdays",
                column: "BirthdayConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_BotConfigId",
                table: "GuildConfigs",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfigs_VoiceChannelConfigId",
                table: "GuildConfigs",
                column: "VoiceChannelConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageConfigs_GuildConfigId",
                table: "ImageConfigs",
                column: "GuildConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoggingConfigs_GuildConfigId",
                table: "LoggingConfigs",
                column: "GuildConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTrackingConfigs_GuildConfigId",
                table: "OrderTrackingConfigs",
                column: "GuildConfigId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Birthdays");

            migrationBuilder.DropTable(
                name: "ImageConfigs");

            migrationBuilder.DropTable(
                name: "LoggingConfigs");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderTrackingConfigs");

            migrationBuilder.DropTable(
                name: "BirthdayConfigs");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "VoiceChannelConfig");
        }
    }
}
