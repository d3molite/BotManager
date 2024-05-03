using BotManager.ImageProcessing.Enum;
using BotManager.ImageProcessing.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BotManager.ImageProcessing.Commands;

public static class MirrorCommands
{
    public static async Task ExecuteMirror(ImageModel imageModel, MirrorDirection direction)
    {
        var image = await Image.LoadAsync(imageModel.SourcePath);

        if (image.Frames.Count > 1)
        {
            for (int i = 0; i < image.Frames.Count; i++)
            {
                using var frame = image.Frames.ExportFrame(i);
                ProcessMirror(frame, direction);
                image.Frames.InsertFrame(i, frame.Frames.RootFrame);
            }
        }
        
        else
            ProcessMirror(image, direction);
        
        await image.SaveAsync(imageModel.TargetPath);
    
        image.Dispose();
    }


    private static void ProcessMirror(Image image, MirrorDirection direction)
    {
        var size = image.Size;
        
        var crop = image.Clone(
            context =>
            {
                context.Crop(GetCropArea(size, direction));
                context.Flip(GetFlipMode(direction));
            });
        
        image.Mutate(context =>
        {
            context.DrawImage(crop, GetPlacementTarget(size, direction), opacity: 1);
        });
    }

    private static Rectangle GetCropArea(Size size, MirrorDirection direction)
    {
        var width = size.Width;
        var height = size.Height;

        return direction switch
        {
            MirrorDirection.LeftToRight => new Rectangle(0, 0, width / 2, height),
            MirrorDirection.RightToLeft => new Rectangle(width / 2, 0, width / 2, height),
            MirrorDirection.TopToBottom => new Rectangle(0, 0, width, height / 2),
            MirrorDirection.BottomToTop => new Rectangle(0, height / 2, width, height / 2),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private static FlipMode GetFlipMode(MirrorDirection direction)
    {
        return direction switch
        {
            MirrorDirection.LeftToRight => FlipMode.Horizontal,
            MirrorDirection.RightToLeft => FlipMode.Horizontal,
            MirrorDirection.TopToBottom => FlipMode.Vertical,
            MirrorDirection.BottomToTop => FlipMode.Vertical,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private static Point GetPlacementTarget(Size size, MirrorDirection direction)
    {
        var width = size.Width;
        var height = size.Height;

        return direction switch
        {
            MirrorDirection.LeftToRight => new Point(width / 2, 0),
            MirrorDirection.RightToLeft => new Point(0, 0),
            MirrorDirection.TopToBottom => new Point(0, height / 2),
            MirrorDirection.BottomToTop => new Point(0, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}