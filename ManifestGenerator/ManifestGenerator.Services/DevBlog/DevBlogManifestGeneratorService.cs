using System.Globalization;
using System.Text.RegularExpressions;
using ManifestGenerator.Common.Files;
using ManifestGenerator.Models.DevBlog;
using ManifestGenerator.Services.DevBlog.Interfaces;
using Newtonsoft.Json;

namespace ManifestGenerator.Services.DevBlog;

/// <summary>
///   Service to generate a manifest for the DevBlog
/// </summary>
public class DevBlogManifestGeneratorService : IDevBlogManifestGeneratorService
{
    /// <summary>
    ///  Generate a manifest for the DevBlog using the default directory and developer blog folder name
    /// </summary>
    /// <returns></returns>
    public async Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync( bool useDefaultFolderName = true, string? devBlogFolderName = null )
    {
        string targetDirectory = string.Empty;
        switch (devBlogFolderName)
        {
            case null when useDefaultFolderName:
                targetDirectory = Path.Combine( Directory.GetCurrentDirectory(), "DevBlog");
                return await GenerateManifestAsync(targetDirectory);
            case null:
                return (false, string.Empty, string.Empty);
        }

        targetDirectory = Path.Combine( Directory.GetCurrentDirectory(), devBlogFolderName);;
        return await GenerateManifestAsync(targetDirectory);

    }

    /// <summary>
    ///  Generate a manifest for the DevBlog
    /// </summary>
    /// <param name="targetDirectory"></param>
    /// <returns></returns>
    public async Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory)
    {
        var manifestFileName = "Dev_Blog_Manifest.json";
        return await GenerateManifestAsync(targetDirectory, manifestFileName);
    }

    /// <summary>
    ///  Generate a manifest for the DevBlog
    /// </summary>
    /// <param name="targetDirectory"></param>
    /// <param name="manifestFileName"></param>
    /// <returns></returns>
    public async Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory, string manifestFileName)
    {
        return await GenerateManifestAsync(targetDirectory, manifestFileName, targetDirectory);
    }

    /// <summary>
    ///  Generate a manifest for the DevBlog
    /// </summary>
    /// <param name="targetDirectory"></param>
    /// <param name="manifestFileName"></param>
    /// <param name="blogPostDirectory"></param>
    /// <returns></returns>
    public async Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory, string manifestFileName, string blogPostDirectory)
    {
        return await GenerateManifestAsync(targetDirectory, manifestFileName, blogPostDirectory, true);
    }
    
    /// <summary>
    ///  Generate a manifest for the DevBlog
    /// </summary>
    /// <param name="targetDirectory"></param>
    /// <param name="manifestFileName"></param>
    /// <param name="blogPostDirectory"></param>
    /// <param name="saveFile"></param>
    /// <returns></returns>
    public async Task<(bool success, string manifestJson, string outputPath)> GenerateManifestAsync(string targetDirectory, string manifestFileName, string blogPostDirectory, bool saveFile)
    {
        bool success = false;
        string manifestJson = string.Empty;
        string outputPath = string.Empty;
        try
        { 
            // Ensure the target directory exists
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            
            // Ensure the blog post directory exists
            if (!Directory.Exists(blogPostDirectory))
            {
                Directory.CreateDirectory(blogPostDirectory);
            }
            
            var devBlogPosts = new List<BlogPost>();
            
            // get all year directories
            var yearDirectories = Directory.GetDirectories(blogPostDirectory);

            foreach (var yearDirectory in yearDirectories)
            {
                // Get all month directories within the year directory
                var monthDirectories = Directory.GetDirectories(yearDirectory);

                foreach (var monthDirectory in monthDirectories)
                {
                    // Get all blog post directories within the month directory
                    var blogPostDirectories = Directory.GetDirectories(monthDirectory);

                    foreach (var postDirectory in blogPostDirectories)
                    {
                        var postInfo = new DirectoryInfo(postDirectory);
                        var post = new BlogPost
                        {
                            Title = postInfo.Name,
                            ContentFiles = new List<string>(),
                            ImageFiles = new List<string>(),
                            Date = ExtractDateFromFolderName(postInfo.Name)
                        };

                        var files = Directory.GetFiles(postDirectory).OrderBy(name => name).ToList();
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileName(file);
                            var extension = Path.GetExtension(file).ToLowerInvariant();
                            // Get the relative path with respect to the 'wwwroot' directory
                            var relativePath = Path.GetRelativePath(blogPostDirectory, file).Replace("\\", "/"); // Convert to web-friendly path

                            if (FileTypes.DocumentExtensions.Contains(extension))
                            {
                                post.ContentFiles.Add(relativePath); // Prepend 'blog/' to the relative path
                            }
                            else if (FileTypes.ImageExtensions.Contains(extension))
                            {
                                post.ImageFiles.Add(relativePath); // Prepend 'blog/' to the relative path
                            }
                        }

                        devBlogPosts.Add(post);
                    }
                }
            }
            manifestJson = JsonConvert.SerializeObject(devBlogPosts, Formatting.Indented);
            outputPath = Path.Combine(targetDirectory, manifestFileName);
            if (saveFile)
            {
                await SaveManifestAsync(manifestJson, targetDirectory, manifestFileName);
            }
            success = true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to generate manifest: {e.Message}");
            return (false, string.Empty, string.Empty);
        }
        return (success, manifestJson, outputPath);
    }
    
    /// <summary>
    ///  Extract the date from the folder name in the format 'yyyy-MM-dd'
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public DateTime ExtractDateFromFolderName(string folderName)
    {
        var regex = new Regex(@"(\d{4}-\d{2}-\d{2})");
        var match = regex.Match(folderName);
        if (match.Success)
        {
            return DateTime.ParseExact(match.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        return DateTime.MinValue; // Or a sensible default
    }

    /// <summary>
    ///  Save the manifest to the target directory with the specified file name
    /// </summary>
    /// <param name="manifest"></param>
    /// <param name="targetDirectory"></param>
    /// <param name="manifestFileName"></param>
    /// <returns></returns>
    public async Task<(bool success, string outputPath)> SaveManifestAsync(string manifest, string targetDirectory, string manifestFileName)
    {
        bool success = false;
        string outputPath = string.Empty;
        try
        {
            outputPath = Path.Combine(targetDirectory, manifestFileName);
            await File.WriteAllTextAsync(outputPath, manifest);
            success = true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error trying to save {manifestFileName} to {targetDirectory}: {e.Message}");
            return (false, string.Empty);
        }
        return (success, outputPath);
    }
}