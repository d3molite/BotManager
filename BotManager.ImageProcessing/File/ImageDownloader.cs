using BotManager.ImageProcessing.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace BotManager.ImageProcessing.File;

public static class ImageDownloader
{
    private static readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "image");
    
    private static string CleanImageUrl(string url) => url.Split("?").First();
    
    public static async Task<ImageModel> Download(string url)
    {
        var clean = CleanImageUrl(url);
        var extension = clean.Split('.').Last();

        var image = new ImageModel("image", extension, _path);

        using var client = new HttpClient();

        if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

        if (!Directory.Exists(image.SubFolder)) Directory.CreateDirectory(image.SubFolder);

        await using var s = await client.GetStreamAsync(new Uri(url));

        await using var fs = new FileStream(image.SourcePath, FileMode.Create);

        await s.CopyToAsync(fs);
        
        await fs.DisposeAsync();
        await s.DisposeAsync();
        

        if (image.Extension == "webp")
            await ConvertToJpegAsync(image);

        await ResizeToManageable(image);

        return image;
    }

    private static readonly JpegEncoder _encoder = new()
    {
        Quality = 100,
    };

    private static async Task ConvertToJpegAsync(ImageModel imageModel)
    {
        using var image = await Image.LoadAsync(imageModel.SourcePath);

        imageModel.Extension = "jpg";

        await image.SaveAsJpegAsync(imageModel.SourcePath, _encoder);
    }

    private static async Task ResizeToManageable(ImageModel imageModel)
    {
        using var image = await Image.LoadAsync(imageModel.SourcePath);

        if (image.Width < 1920) return;
        
        image.Mutate(
            x => x.Resize(1920,0)
            );

        await image.SaveAsync(imageModel.SourcePath);
    }
}