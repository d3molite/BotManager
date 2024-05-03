using BotManager.ImageProcessing.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace BotManager.ImageProcessing.Commands;

public static class DestructionCommands
{
    public static async Task ExecuteJpeg(ImageModel imageModel)
    {
        var image = await Image.LoadAsync(imageModel.SourcePath);

        if (image.Frames.Count > 1)
        {
            for (int i = 0; i < image.Frames.Count; i++)
            {
                using var frame = image.Frames.ExportFrame(i);
                ProcessJpeg(frame);
                image.Frames.InsertFrame(i, frame.Frames.RootFrame);
            }
        }
        
        else
            ProcessJpeg(image);

        imageModel.Extension = "jpg";
        
        await image.SaveAsJpegAsync(imageModel.TargetPath, new JpegEncoder()
        {
            Quality = 1,
        });
        
        image.Dispose();
    }

    private static void ProcessJpeg(Image image)
    {
        var height = image.Height;
        var newHeight = image.Height / 3;
        
        image.Mutate(context => context.Resize(height: newHeight, width:0));
        
        image.Mutate(context =>
        {
            context.Saturate(1.25f);
        });
        
        image.Mutate(context => context.Resize(height: height, width:0));
    }
}