using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PriceComparisonWebsite.Services.Utilities.Interfaces;

namespace PriceComparisonWebsite.Services.Utilities
{
    public class RobotsTxtChecker : IRobotsTxtChecker
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RobotsTxtChecker> _logger;
        private readonly string _cacheFolder;
        private readonly IFileSystemWrapper _fileSystem; 

        public RobotsTxtChecker(HttpClient httpClient, ILogger<RobotsTxtChecker> logger, IFileSystemWrapper fileSystem, string cacheFolder = "RobotsCache")
        {
            _httpClient = httpClient;
            _logger = logger;
            _cacheFolder = cacheFolder;
            _fileSystem = fileSystem;

            // Ensure cache folder exists
            _fileSystem.CreateDirectory(_cacheFolder);
        }

        public async Task<bool> CheckRobotsTxt(Uri url)
        {
            try
            {
                string domain = url.Host;

                // Cache robots.txt if not already cached
                if (!CheckIfRobotsTxtFileIsCached(domain))
                {
                    await CacheRobotsTxtFile(url);
                }

                string filePath = Path.Combine(_cacheFolder, $"{domain}_robots.txt");

                if (!_fileSystem.FileExists(filePath))
                {
                    return false;  // No robots.txt -> assume disallow
                }

                var content = await _fileSystem.ReadAllTextAsync(filePath);

                // Parse robots.txt file
                var disallowRules = new List<string>();
                var currentUserAgent = "*";  // Default to global rules

                var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                        continue;

                    var parts = trimmedLine.Split(new[] { ':' }, 2);
                    if (parts.Length != 2)
                        continue;

                    var directive = parts[0].Trim().ToLower();
                    var value = parts[1].Trim();

                    if (directive == "user-agent")
                    {
                        currentUserAgent = value.ToLower();
                    }
                    else if (directive == "disallow" && currentUserAgent == "*")
                    {
                        disallowRules.Add(value);
                    }
                }

                string path = url.AbsolutePath;

                // Check if the path is disallowed
                bool isAllowed = !disallowRules.Any(rule => path.StartsWith(rule, StringComparison.OrdinalIgnoreCase));

                _logger.LogInformation($"Robots.txt check for {url} => {(isAllowed ? "Allowed" : "Disallowed")}");

                return isAllowed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking robots.txt for {Url}", url);
                return false;  // Default to disallowed on errors
            }
        }

        public async Task CacheRobotsTxtFile(Uri url)
        {
            try
            {
                var domain = url.Host;
                var filePath = Path.Combine(_cacheFolder, $"{domain}_robots.txt");
                var robotsUrl = new Uri($"{url.Scheme}://{domain}/robots.txt");

                using var response = await _httpClient.GetAsync(robotsUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to download robots.txt for {domain}. Status: {response.StatusCode}");
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();

                // Log whether caching or overwriting
                if (_fileSystem.FileExists(filePath))
                {
                    _logger.LogInformation($"Overwriting existing robots.txt for {domain}.");
                }
                else
                {
                    _logger.LogInformation($"Caching new robots.txt for {domain}.");
                }

                // Save the file
                await _fileSystem.WriteAllTextAsync(filePath, content);

                _logger.LogInformation($"Cached robots.txt for {domain} at {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching robots.txt for {Url}", url);
            }
        }

        public bool CheckIfRobotsTxtFileIsCached(string domain)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(domain))
                {
                    _logger.LogWarning("Domain is null or empty.");
                    return false;
                }

                string normalizedDomain = domain.Replace("www.", "");
                string filePath = Path.Combine(_cacheFolder, $"{normalizedDomain}_robots.txt");

                if (!_fileSystem.FileExists(filePath))
                {
                    return false;  // No cache file found
                }

                var lastWriteTime = _fileSystem.GetLastWriteTime(filePath);
                var age = DateTime.Now - lastWriteTime;

                const int cacheExpirationHours = 24;
                if (age.TotalHours > cacheExpirationHours)
                {
                    return false;  // Cache expired
                }

                return true;  // Cache is valid
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache for robots.txt.");
                return false;  // Default to not cached on errors
            }
        }
    }
}
