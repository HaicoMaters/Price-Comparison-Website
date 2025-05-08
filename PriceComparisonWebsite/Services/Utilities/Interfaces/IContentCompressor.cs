using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    public interface IContentCompressor
    {
        Task<string> DecompressAsync(byte[] compressedContent);
        Task<byte[]> CompressAsync(string content);
    }
}