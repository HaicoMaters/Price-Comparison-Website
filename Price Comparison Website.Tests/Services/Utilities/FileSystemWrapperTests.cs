using Price_Comparison_Website.Services.Utilities;

namespace Price_Comparison_Website.Tests.Services.Utilities
{
    public class FileSystemWrapperTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFile;
        private readonly FileSystemWrapper _wrapper;

        public FileSystemWrapperTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "FileSystemWrapperTests");
            _testFile = Path.Combine(_testDirectory, "test.txt");

            _wrapper = new FileSystemWrapper();

            // Ensure clean state
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        // -------------------------------------------------------- FileExists ---------------------------------------------------------------------------------------

        [Fact]
        public void FileExists_WhenFileExists_ShouldReturnTrue()
        {
            // Arrange
            File.WriteAllText(_testFile, "Test content");

            // Act
            var result = _wrapper.FileExists(_testFile);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void FileExists_WhenFileDoesNotExist_ShouldReturnFalse()
        {
            // Act
            var result = _wrapper.FileExists(_testFile);

            // Assert
            Assert.False(result);
        }

        // -------------------------------------------------------- GetLastWriteTime ---------------------------------------------------------------------------------------


        [Fact]
        public void GetLastWriteTime_WhenFileExists_ShouldReturnCorrectTime()
        {
            // Arrange
            File.WriteAllText(_testFile, "Test content");
            var expectedTime = File.GetLastWriteTime(_testFile);

            // Act
            var result = _wrapper.GetLastWriteTime(_testFile);

            // Assert
            Assert.Equal(expectedTime, result);
        }

        [Fact]
        public void GetLastWriteTime_WhenFileDoesNotExist_ShouldReturnMinValue()
        {
            // Act
            var result = _wrapper.GetLastWriteTime(_testFile);

            // Assert
            Assert.True(result == DateTime.MinValue || result == new DateTime(1601, 1, 1)); // Pass For both windows and linux
        }

        // -------------------------------------------------------- WriteAllTextAsync ---------------------------------------------------------------------------------------


        [Fact]
        public async Task WriteAllTextAsync_ShouldWriteCorrectContent()
        {
            // Act
            await _wrapper.WriteAllTextAsync(_testFile, "Hello, World!");

            // Assert
            var content = await File.ReadAllTextAsync(_testFile);
            Assert.Equal("Hello, World!", content);
        }

        [Fact]
        public async Task WriteAllTextAsync_ShouldOverwriteExistingContent()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFile, "Old Content");

            // Act
            await _wrapper.WriteAllTextAsync(_testFile, "New Content");

            // Assert
            var content = await File.ReadAllTextAsync(_testFile);
            Assert.Equal("New Content", content);
        }

        // -------------------------------------------------------- ReadAllTextAsync ---------------------------------------------------------------------------------------

        [Fact]
        public async Task ReadAllTextAsync_WhenFileExists_ShouldReturnCorrectContent()
        {
            // Arrange
            await File.WriteAllTextAsync(_testFile, "Read this content");

            // Act
            var content = await _wrapper.ReadAllTextAsync(_testFile);

            // Assert
            Assert.Equal("Read this content", content);
        }

        [Fact]
        public async Task ReadAllTextAsync_WhenFileDoesNotExist_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _wrapper.ReadAllTextAsync(_testFile));
        }

        // --------------------------------------------------------- CreateDirectory ------------------------------------------------------------------------------------


        [Fact]
        public void CreateDirectory_WhenDirectoryDoesNotExist_ShouldCreateIt()
        {
            // Arrange
            var newDirectory = Path.Combine(_testDirectory, "NewFolder");

            // Act
            _wrapper.CreateDirectory(newDirectory);

            // Assert
            Assert.True(Directory.Exists(newDirectory));
        }

        [Fact]
        public void CreateDirectory_WhenDirectoryExists_ShouldNotThrowException()
        {
            // Act
            _wrapper.CreateDirectory(_testDirectory);

            // Assert
            Assert.True(Directory.Exists(_testDirectory));
        }
    }
}