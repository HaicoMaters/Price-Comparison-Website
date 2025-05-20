using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    /// <summary>
    /// Wrapper for file system operations to enable unit testing
    /// </summary>
    public interface IFileSystemWrapper
    {
        /// <summary>
        /// Checks if a file exists at the specified path
        /// </summary>
        bool FileExists(string path);

        /// <summary>
        /// Gets the last write time of a file
        /// </summary>
        DateTime GetLastWriteTime(string path);

        /// <summary>
        /// Writes text content to a file asynchronously
        /// </summary>
        Task WriteAllTextAsync(string path, string content);

        /// <summary>
        /// Reads all text from a file asynchronously
        /// </summary>
        Task<string> ReadAllTextAsync(string path);

        /// <summary>
        /// Creates a directory if it doesn't exist
        /// </summary>
        void CreateDirectory(string path);
    }
}