using System;
using System.IO;
using System.IO.Compression;

namespace GZipArchiver
{
    public class GZipCompressor : GZip
    {
        private const int BlockSize = 1024 * 1024 * 4;

        public GZipCompressor(string inputPath)
            : base(inputPath)
        {
            _outputPath = Path.ChangeExtension(_inputPath, Path.GetExtension(_inputPath) + ".gz");
        }
        public GZipCompressor(string inputPath, string outputPath)
            : base(inputPath, outputPath) { }

        protected override BlockReader CreateReader(Stream stream)
        {
            return new CompressBlockReader(stream, BlockSize);
        }

        protected override byte[] Process(byte[] blockBytes)
        {
            try
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        zipStream.Write(blockBytes, 0, blockBytes.Length);
                    }
                    var compressedBytes = compressedStream.ToArray();
                    SetLengthIntoBlock(compressedBytes);
                    return compressedBytes;
                }
            }
            catch
            {
                throw new Exception("Произошла ошибка при компрессии одного из блоков!");
            }
        }

        private void SetLengthIntoBlock(byte[] block)
        {
            BitConverter.GetBytes(block.Length).CopyTo(block, 4);
        }
    }
}
