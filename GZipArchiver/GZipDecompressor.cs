using System;
using System.IO;
using System.IO.Compression;

namespace GZipArchiver
{
    public class GZipDecompressor : GZip
    {
        public GZipDecompressor(string inputPath)
            : base(inputPath)
        {
            _outputPath =
                Path.GetDirectoryName(_inputPath) +
                Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(_inputPath);
        }
        public GZipDecompressor(string inputPath, string outputPath)
            : base(inputPath, outputPath) { }

        protected override BlockReader CreateReader(Stream stream)
        {
            return new DecompressBlockReader(stream);
        }
        protected override byte[] Process(byte[] blockBytes)
        {
            try
            {
                using (var stream = new MemoryStream(blockBytes))
                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                using (var decompressedStream = new MemoryStream())
                {
                    var buffer = new byte[BitConverter.ToInt32(blockBytes, blockBytes.Length - 4)];
                    zipStream.Read(buffer, 0, buffer.Length);
                    decompressedStream.Write(buffer, 0, buffer.Length);
                    return decompressedStream.ToArray();
                }
            }
            catch
            {
                throw new Exception("Произошла ошибка при декомпрессии одного из блоков!");
            }
        }
    }
}
