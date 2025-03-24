using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price_Comparison_Website.Services.Utilities.Interfaces
{
    /// A wrapper around the file system operations to allow for easier testing and abstraction.
    /// This interface provides methods to interact with files and directories without directly using System.IO.
    /// Allows for mocking file operations during unit tests,

    public interface IFileSystemWrapper
    {
        bool FileExists(string path);
        DateTime GetLastWriteTime(string path);
        Task WriteAllTextAsync(string path, string content);
        Task<string> ReadAllTextAsync(string path);
        void CreateDirectory(string path);
    }
}