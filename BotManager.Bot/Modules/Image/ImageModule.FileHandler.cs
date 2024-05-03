using BotManager.ImageProcessing.File;
using BotManager.ImageProcessing.Model;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace BotManager.Bot.Modules.Image;

public partial class ImageModule
{
    private readonly List<string> _formats = ["jpg", "png", "bmp", "jpeg", "webp", "gif"];
    
    private async Task<ImageResult> TryGetImage(SocketInteraction interaction)
    {
        var image = await GetImageLink(interaction);

        if (image is null)
        {
            await interaction.FollowupAsync("No Image found.", ephemeral: true);
            return ImageResult.Failed();
        }

        try
        {
            var imageObject = await ImageDownloader.Download(image);
            return ImageResult.Ok(imageObject);
        }
        catch (Exception ex)
        {
            Log.Error("Exception in Downloading Image. {exception}", ex);
            await interaction.FollowupAsync("Error downloading image.");
            return ImageResult.Failed();
        }
    }
    
    private async Task<string?> GetImageLink(SocketInteraction interaction)
    {
        var channel = interaction.Channel;
        var messages = (await channel.GetMessagesAsync(10).FlattenAsync()).ToList();

        foreach (var message in messages)
        {
            if (message.Embeds.Count != 0)
            {
                var images = message.Embeds.Where(embed => embed.Url != null).ToArray();

                if (images.Length != 0)
                {
                    var image = images.First();
                    
                    if (IsImage(image.Url)) 
                        return image.Url;
                }
            }

            if (message.Attachments.Count == 0) continue;
            {
                var images = message.Attachments.Where(attachment => attachment.Url != null);

                var image = images.First();

                if (IsImage(image.Url)) 
                    return image.Url;
            }
        }

        return string.Empty;
    }
    
    private async Task SendAndDelete(ImageModel imageModel, SocketInteraction interaction)
    {
        try
        {
            await interaction.FollowupWithFileAsync(imageModel.FinalPath);
        }
        catch (Exception ex)
        {
            await interaction.FollowupAsync("The resulting file was too large.", ephemeral: true);
        }

        Directory.Delete(imageModel.SubFolder, true);
    }
    
    private bool IsImage(string url)
    {
        var imageExt = CleanImageUrl(url).Split('.').Last().ToLower();
        return _formats.Contains(imageExt);
    }
    
    private static string CleanImageUrl(string url) => url.Split("?").First();
}