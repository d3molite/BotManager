using BotManager.Bot.Modules.Definitions;
using BotManager.ImageProcessing.Commands;
using Discord;
using Discord.WebSocket;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    private async Task BuildJpegCommand(ulong guildId)
    {
        var guild = client.GetGuild(guildId);

        var command = new SlashCommandBuilder()
            .WithName(Commands.NeedsMoreJpeg)
            .WithDescription("Mmmm crusty")
            .AddDescriptionLocalization("de", "KNUSPRIG");

        await guild.CreateApplicationCommandAsync(command.Build());
    }

    private async Task ExecuteMoreJpegCommand(SocketSlashCommand command)
    {
        var result = await DeferAndTryGet(command);

        if (result is not null)
        {
            var file = result.ImageModel!;
            
            await DestructionCommands.ExecuteJpeg(file);
            await SendAndDelete(file, command);
        }
    }
}