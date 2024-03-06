using ManifestGenerator.Services.DevBlog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ManifestGenerator.ConsoleView
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            bool exit = false;
            bool headless = false;
            string? blogPostFolderName = "DevBlog";
            string? targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), blogPostFolderName);
            string? blogPostDirectory = Path.Combine(Directory.GetCurrentDirectory(), blogPostFolderName);
            string manifestName = "Dev_Blog_Manifest.json"; // Default manifest name

            // Manual argument parsing
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--target-directory":
                    case "-t":
                        targetDirectory = i + 1 < args.Length ? args[++i] : null;
                        break;
                    case "--blog-post-directory":
                    case "-b":
                        blogPostDirectory = i + 1 < args.Length ? args[++i] : null;
                        break;
                    case "--blog-folder-name":
                    case "-f":
                        blogPostFolderName = i + 1 < args.Length ? args[++i] : null;
                        break;
                    case "--manifest-name":
                    case "-m":
                        manifestName = i + 1 < args.Length ? args[++i] : manifestName;
                        break;
                    case "--headless":
                    case "-h":
                        headless = true;
                        break;
                    default:
                        Console.WriteLine($"Invalid argument: {args[i]}");
                        break;

                }
            }

            if (headless)
            {
                // Use defaults if not specified
                targetDirectory ??= Path.Combine(Directory.GetCurrentDirectory(), blogPostFolderName);
                blogPostDirectory ??= targetDirectory; // Assumes blog posts are within the target directory

                await GenerateManifest(targetDirectory, blogPostDirectory, manifestName);
                return;
            }

            while (!exit)
            {
                // Display current configuration
                Console.Clear();
                ShowCurrentConfiguration(targetDirectory, blogPostDirectory, blogPostFolderName, manifestName);

                // Options menu
                ShowMenu();
                var selectedOption = Console.ReadLine();

                switch (selectedOption)
                {
                    case "1":
                        targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), blogPostFolderName);
                        blogPostDirectory = targetDirectory;
                        manifestName = "Dev_Blog_Manifest.json";
                        await GenerateManifest(targetDirectory, blogPostDirectory, manifestName);
                        exit = true;
                        break;
                    case "2":
                        Console.Write("Enter Custom Blog Folder Name: ");
                        blogPostDirectory = Console.ReadLine();
                        break;
                    case "3":
                        Console.Write("Enter Target Directory: ");
                        targetDirectory = Console.ReadLine();
                        break;
                    case "4":
                        Console.Write("Enter Blog Folder Directory: ");
                        blogPostDirectory = Console.ReadLine();
                        break;
                    case "5":
                        Console.Write("Enter Manifest File Name: ");
                        manifestName = Console.ReadLine();
                        break;
                    case "6":
                        await GenerateManifest(targetDirectory, blogPostDirectory, manifestName);
                        exit = true;
                        break;
                    case "7":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option selected. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void ShowCurrentConfiguration(string? targetDirectory, string? blogPostDirectory, string? blogFolderName, string manifestName)
        {
            Console.WriteLine("Current Configuration:");
            Console.WriteLine($"  Target Directory: {targetDirectory ?? "Not specified"} [{FolderExists(targetDirectory)}]");
            Console.WriteLine($"  Blog Folder Directory: {blogPostDirectory ?? "Not specified"} [{FolderExists(blogPostDirectory)}]");
            var folderNameInfo = string.IsNullOrWhiteSpace(blogFolderName) ? "Invalid Folder Name" : "Valid Folder Name";
            Console.WriteLine($"  Blog Folder Name: {blogFolderName ?? "Not specified"} [{folderNameInfo}]");
            var manifestInfo = string.IsNullOrWhiteSpace(manifestName) ? "Invalid Manifest Name" : "Valid Manifest Name";
            Console.WriteLine($"  Manifest Name: {manifestName ?? "Not specified"} [{manifestInfo}]\n");

        }

        private static void ShowMenu()
        {
            Console.WriteLine("Select Manifest Generator Mode:");
            Console.WriteLine("1. Default");
            Console.WriteLine("2. Specify Target Directory");
            Console.WriteLine("3. Custom Blog Folder Name");
            Console.WriteLine("4. Specify Blog Folder Directory");
            Console.WriteLine("5. Specify Manifest File Name");
            Console.WriteLine("6. Generate Manifest");
            Console.WriteLine("7. Exit");
            Console.Write("Enter option: ");
        }

        private static string FolderExists(string? directory)
        {
            return !string.IsNullOrEmpty(directory) && Directory.Exists(directory) ? "Exists" : "Does Not Exist";
        }

        private static async Task<(bool success, string manifestJson, string outputPath)> GenerateManifest(string targetDirectory, string blogPostDirectory, string manifestName)
        {
            var service = new DevBlogManifestGeneratorService();
            var result = await service.GenerateManifestAsync(targetDirectory, manifestName, blogPostDirectory, true);
            Console.WriteLine(result.success ? $"Manifest generated at {result.outputPath}" : $"Failed to generate manifest at {result.outputPath}");
            return result;
        }
    }
}
