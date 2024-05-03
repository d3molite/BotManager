using BotManager.ImageProcessing.Model;
using SixLabors.ImageSharp;

namespace BotManager.ImageProcessing.Commands;

public static class GifCommands
{
    public static async Task ExecuteReverse(ImageModel imageModel)
    {
        var image = await Image.LoadAsync(imageModel.SourcePath);
        
        if (image.Frames.Count > 1)
        {
            var frames = new List<ImageFrame>();
            var count = image.Frames.Count;

            while (image.Frames.Count > 1)
            {
                var frame = image.Frames.ExportFrame(0);
                frames.Add(frame.Frames.RootFrame);
            }
            
            frames.Reverse();

            for (int i = 0; i < frames.Count; i++)
            {
                var frame = frames[i];
                image.Frames.InsertFrame(i, frame);
            }
        }
        
        await image.SaveAsync(imageModel.TargetPath);
    
        image.Dispose();
    }
}