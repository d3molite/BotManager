using BotManager.Bot.Modules.Definitions;
using BotManager.Db.Models.Modules.Image;
using Discord;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    public async Task BuildCommands(ImageConfig config, ulong guildId)
    {
        await BuildWaawCommand(guildId);
        await BuildHaahCommand(guildId);
        await BuildWoowCommand(guildId);
        await BuildHoohCommand(guildId);
    }

    private async Task BuildWaawCommand(ulong guildId)
    {
        var guild = client.GetGuild(guildId);

        var command = new SlashCommandBuilder();
        command.WithName(Commands.Waaw);
        command.WithDescription("Mirror image left to right");
        command.AddDescriptionLocalization("de", "Spiegel das letzte Bild von links nach rechts");

        await guild.CreateApplicationCommandAsync(command.Build());
    }
    
    private async Task BuildWoowCommand(ulong guildId)
    {
        var guild = client.GetGuild(guildId);

        var command = new SlashCommandBuilder();
        command.WithName(Commands.Woow);
        command.WithDescription("Mirror image right to left");
        command.AddDescriptionLocalization("de", "Spiegel das letzte Bild von rechts nach links");

        await guild.CreateApplicationCommandAsync(command.Build());
    }
    
    private async Task BuildHaahCommand(ulong guildId)
    {
        var guild = client.GetGuild(guildId);

        var command = new SlashCommandBuilder();
        command.WithName(Commands.Haah);
        command.WithDescription("Mirror image top to bottom");
        command.AddDescriptionLocalization("de", "Spiegel das letzte Bild von oben nach unten");

        await guild.CreateApplicationCommandAsync(command.Build());
    }
    
    private async Task BuildHoohCommand(ulong guildId)
    {
        var guild = client.GetGuild(guildId);

        var command = new SlashCommandBuilder();
        command.WithName(Commands.Hooh);
        command.WithDescription("Mirror image bottom to top");
        command.AddDescriptionLocalization("de", "Spiegel das letzte Bild von unten nach oben");

        await guild.CreateApplicationCommandAsync(command.Build());
    }
}