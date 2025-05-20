using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    /// <summary>
    /// Handles compression and decompression of content
    /// </summary>
    public interface IContentCompressor
    {
        /// <summary>
        /// Decompresses GZIP compressed content
        /// </summary>
        /// <param name="compressedContent">The compressed byte array</param>
        /// <returns>The decompressed string content</returns>
        Task<string> DecompressAsync(byte[] compressedContent);

        /// <summary>
        /// Compresses string content using GZIP
        /// </summary>
        /// <param name="content">The string content to compress</param>
        /// <returns>The compressed byte array</returns>
        Task<byte[]> CompressAsync(string content);
    }
}