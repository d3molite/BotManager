namespace BotManager.ImageProcessing.Model;

public class ImageResult
{
    public ImageModel? ImageModel { get; set; }

    public bool Success { get; set; }

    public static ImageResult Failed()
    {
        return new ImageResult
        {
            Success = false
        };
    }

    public static ImageResult Ok(ImageModel image)
    {
        return new ImageResult
        {
            Success = true,
            ImageModel = image
        };
    }
}