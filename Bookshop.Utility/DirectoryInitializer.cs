using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Bookshop.Utility
{
    public interface IDirectoryInitializer
    {
        void EnsureDirectoriesExist();
    }

    public class DirectoryInitializer : IDirectoryInitializer
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DirectoryInitializer> _logger;

        // Define required directories
        private static readonly string[] RequiredDirectories =
        {
            @"Images\Branding",
            @"Images\Product",
            @"Images\Slides"
        };

        public DirectoryInitializer(IWebHostEnvironment environment, ILogger<DirectoryInitializer> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public void EnsureDirectoriesExist()
        {
            var wwwrootPath = _environment.WebRootPath;

            foreach (var directory in RequiredDirectories)
            {
                var fullPath = Path.Combine(wwwrootPath, directory);

                if (!Directory.Exists(fullPath))
                {
                    try
                    {
                        Directory.CreateDirectory(fullPath);
                        _logger.LogInformation("Created directory: {Directory}", fullPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create directory: {Directory}", fullPath);
                        throw;
                    }
                }
                else
                {
                    _logger.LogDebug("Directory already exists: {Directory}", fullPath);
                }
            }
        }
    }
}