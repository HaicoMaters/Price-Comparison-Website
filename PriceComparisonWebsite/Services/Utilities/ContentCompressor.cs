using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using PriceComparisonWebsite.Services.Utilities.Interfaces;

namespace PriceComparisonWebsite.Services.Utilities
{
    public class ContentCompressor : IContentCompressor
    {
        public async Task<string> DecompressAsync(byte[] compressedContent)
        {
            using (var compressedStream = new MemoryStream(compressedContent))
            using (var decompressStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(decompressStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<byte[]> CompressAsync(string content)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                using (var writer = new StreamWriter(gzipStream))
                {
                    await writer.WriteAsync(content);
                }
                return memoryStream.ToArray();
            }
        }
    }
}