namespace ManifestGenerator.Services.DevBlog.Interfaces;

public interface IDevBlogManifestGeneratorService
{
    Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(bool useDefaultFolderName = true, string? devBlogFolderName = null);
    Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory);
    Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory, string manifestFileName);
    Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory, string manifestFileName, string blogPostDirectory);
    Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory, string manifestFileName, string blogPostDirectory, bool saveFile);
    DateTime ExtractDateFromFolderName(string folderName);
    Task<(bool success, string outputPath)> SaveManifestAsync(string manifestJson, string targetDirectory, string manifestFileName);
}