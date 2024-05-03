namespace BotManager.ImageProcessing.Model;

public class ImageModel
{
    private const string FrameExtension = "jpg";

    public ImageModel(string fileName, string extension, string folder)
    {
        FileName = fileName;
        Extension = extension;
        Folder = folder;
    }

    public string FileName { get; set; }
    public string Extension { get; set; }
    public string Folder { get; set; }

    public bool IsManipulated { get; set; }

    public string SubFolder => Path.Combine(Folder, FileName);

    public string FrameFolder => Path.Combine(SubFolder, "Frames");

    public bool IsAnimated => Extension == "gif";

    public string SourcePath => Path.Combine(SubFolder, $"{FileName}.{Extension}");

    public string TargetPath => Path.Combine(SubFolder, $"{FileName}{(IsManipulated ? 0 : "")}.{Extension}");

    public int Frames { get; set; }

    public int Delay { get; set; }

    public string FramePath(int enumerator)
    {
        return Path.Combine(FrameFolder, $"{FileName}{enumerator}.{FrameExtension}");
    }

    public string ModifiedFramePath(int enumerator)
    {
        return Path.Combine(FrameFolder, $"{FileName}0{enumerator}.{FrameExtension}");
    }
}