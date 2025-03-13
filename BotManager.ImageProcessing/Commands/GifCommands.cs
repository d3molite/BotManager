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

    public static async Task ExecuteSpeed(ImageModel imageModel, double factor)
    {
        var image = await Image.LoadAsync(imageModel.SourcePath);
        
        if (image.Frames.Count > 1)
        {
            switch (factor)
            {
                case < 1:
                    SlowDownGif(image, factor);
                    break;
                
                case 1:
                    break;
                
                case > 1:
                    SpeedUpGif(image, factor);
                    break;
            }
            
        }
       
        await image.SaveAsync(imageModel.TargetPath);
    
        image.Dispose();
    }

    private static void SpeedUpGif(Image image, double factor)
    {
        var originalDelay = image.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay;
        
        if (originalDelay > 0)
        {
            var newDelay = (int)Math.Floor(originalDelay / factor);
            
            foreach (var frame in image.Frames)
            {
                var metadata = frame.Metadata.GetGifMetadata();
                metadata.FrameDelay = newDelay;
            }
        }
        else
        {
            var count = 0;
            
            var frames = new List<ImageFrame>();
            
            while (image.Frames.Count > 1)
            {
                var frame = image.Frames.ExportFrame(0);

                if (count % (int)factor == 0) 
                    frames.Add(frame.Frames.RootFrame);
                
                count++;
            }
            
            for (int i = 0; i < frames.Count; i++)
            {
                var frame = frames[i];
                image.Frames.InsertFrame(i, frame);
            }
        }
    }
    
    private static void SlowDownGif(Image image, double factor)
    {
        var originalDelay = image.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay;
        int newDelay;

        if (originalDelay > 0)
            newDelay = (int)Math.Floor(originalDelay / factor);
        
        else
            newDelay = (int)(Math.Floor(1 / factor));

        foreach (var frame in image.Frames)
        {
            var metadata = frame.Metadata.GetGifMetadata();
            metadata.FrameDelay = newDelay;
        }
    }
}