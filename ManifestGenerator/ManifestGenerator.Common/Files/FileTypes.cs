namespace ManifestGenerator.Common.Files;

/// <summary>
///  File types
/// </summary>
public static class FileTypes
{
    /// <summary>
    ///  Document extensions (e.g. .txt, .pdf, .doc, .docx, .odt)
    /// </summary>
    public static HashSet<string> DocumentExtensions =         
    [
        ".txt", ".pdf", ".doc", ".docx", ".odt" // ... other document extensions
    ];
    
    /// <summary>
    ///  Image extensions (e.g. .png, .jpg, .jpeg, .gif, .bmp, .tiff, .svg)
    /// </summary>
    public static readonly HashSet<string> ImageExtensions =
    [
        ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tiff", ".svg" // ... other image extensions
    ];
    
    /// <summary>
    ///  Video extensions (e.g. .mp4, .webm, .ogg, .mov, .avi, .wmv, .flv, .mkv)
    /// </summary>
    public static readonly HashSet<string> VideoExtensions = 
    [
        ".mp4", ".webm", ".ogg", ".mov", ".avi", ".wmv", ".flv", ".mkv"
    ];
}