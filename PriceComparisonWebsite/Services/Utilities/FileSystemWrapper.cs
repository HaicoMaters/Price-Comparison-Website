using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.Utilities.Interfaces;

namespace PriceComparisonWebsite.Services.Utilities
{
    /// <inheritdoc />
    public class FileSystemWrapper : IFileSystemWrapper
    {
        /// <inheritdoc />
        public bool FileExists(string path) => File.Exists(path);

        /// <inheritdoc />
        public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

        /// <inheritdoc />
        public async Task WriteAllTextAsync(string path, string content) =>
            await File.WriteAllTextAsync(path, content);

        /// <inheritdoc />
        public async Task<string> ReadAllTextAsync(string path) =>
            await File.ReadAllTextAsync(path);

        /// <inheritdoc />
        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}