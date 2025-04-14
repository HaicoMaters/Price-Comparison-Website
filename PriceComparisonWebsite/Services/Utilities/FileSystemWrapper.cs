using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.Utilities.Interfaces;

namespace PriceComparisonWebsite.Services.Utilities
{
    public class FileSystemWrapper : IFileSystemWrapper
    {
        public bool FileExists(string path) => File.Exists(path);

        public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

        public async Task WriteAllTextAsync(string path, string content) =>
            await File.WriteAllTextAsync(path, content);

        public async Task<string> ReadAllTextAsync(string path) =>
            await File.ReadAllTextAsync(path);

        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}