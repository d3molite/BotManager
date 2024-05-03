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

    public int ManipulationCount { get; set; }

    public string SubFolder => Path.Combine(Folder, FileName);

    public string FrameFolder => Path.Combine(SubFolder, "Frames");

    public bool IsAnimated => Extension == "gif";

    public string SourcePath => Path.Combine(SubFolder, $"{FileName}.{Extension}");

    public string TargetPath
    {
        get
        {
            ManipulationCount++;
            var total = Path.Combine(SubFolder, $"{FileName}{ManipulationCount}.{Extension}");
            return total;
        }
    }

    public string FinalPath => Path.Combine(SubFolder, $"{FileName}{ManipulationCount}.{Extension}");

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