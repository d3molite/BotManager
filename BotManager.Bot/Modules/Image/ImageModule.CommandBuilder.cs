using BotManager.Bot.Attributes;
using BotManager.Bot.Extensions;
using BotManager.Bot.Modules.Definitions;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    [CommandBuilder(Commands.Gifspeed)]
    public static async Task BuildSpeedCommand(SocketGuild guild)
    {
        var builder = new SlashCommandBuilder();
        builder.WithName(Commands.Gifspeed);
        builder.WithDescription("Set new speed of gif");
        builder.AddDescriptionLocalization("de", "Setze die neue Gif geschwindigkeit");
        builder.AddOption("factor", ApplicationCommandOptionType.Number, "factor", isRequired:true);

        await guild.TryCreateGuildCommand(builder);
    }

    [CommandBuilder(Commands.Waaw)]
    public static async Task BuildWaawCommand(SocketGuild guild)
    {
        var builder = new SlashCommandBuilder();
        builder.WithName(Commands.Waaw);
        builder.WithDescription("Mirror image left to right");
        builder.AddDescriptionLocalization("de", "Spiegel das letzte Bild von links nach rechts");

        await guild.TryCreateGuildCommand(builder);
    }
    
    [CommandBuilder(Commands.Woow)]
    public static async Task BuildWoowCommand(SocketGuild guild)
    {
        var builder = new SlashCommandBuilder();
        builder.WithName(Commands.Woow);
        builder.WithDescription("Mirror image right to left");
        builder.AddDescriptionLocalization("de", "Spiegel das letzte Bild von rechts nach links");

        await guild.TryCreateGuildCommand(builder);
    }
    
    [CommandBuilder(Commands.Haah)]
    public static async Task BuildHaahCommand(SocketGuild guild)
    {
        var builder = new SlashCommandBuilder();
        builder.WithName(Commands.Haah);
        builder.WithDescription("Mirror image top to bottom");
        builder.AddDescriptionLocalization("de", "Spiegel das letzte Bild von oben nach unten");

        await guild.TryCreateGuildCommand(builder);
    }
    
    [CommandBuilder(Commands.Hooh)]
    public static async Task BuildHoohCommand(SocketGuild guild)
    {
        var builder = new SlashCommandBuilder();
        builder.WithName(Commands.Hooh);
        builder.WithDescription("Mirror image bottom to top");
        builder.AddDescriptionLocalization("de", "Spiegel das letzte Bild von unten nach oben");

        await guild.TryCreateGuildCommand(builder);
    }
    
    [CommandBuilder(Commands.Reverse)]
    public static async Task BuildReverseCommand(SocketGuild guild)
    {
        var builder = new SlashCommandBuilder()
            .WithName(Commands.Reverse)
            .WithDescription("Reverse a gif.")
            .AddDescriptionLocalization("de", "Spiele das letzte Gif rückwärts ab");

        await guild.TryCreateGuildCommand(builder);
    }

    [CommandBuilder(Commands.NeedsMoreJpeg)]
    public static async Task BuildJpegCommand(SocketGuild guild)
    {
        var builder = new SlashCommandBuilder()
            .WithName(Commands.NeedsMoreJpeg)
            .WithDescription("Mmmm crusty")
            .AddDescriptionLocalization("de", "KNUSPRIG");

        await guild.TryCreateGuildCommand(builder);
    }
}