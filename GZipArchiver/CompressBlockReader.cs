using System;
using System.IO;

namespace GZipArchiver
{
    public class CompressBlockReader : BlockReader
    {
        private readonly int _blockSize;
        public CompressBlockReader(Stream stream) : base(stream) { _blockSize = 1024 * 1024; }
        public CompressBlockReader(Stream stream, int blockSize) : base(stream) { _blockSize = blockSize; }
        public override Block ReadBlock()
        {
            try
            {
                lock (_locker)
                {
                    var blockSize = (int) ((_stream.Length - _stream.Position >= _blockSize)
                        ? _blockSize
                        : _stream.Length - _stream.Position);
                    var block = new byte[blockSize];
                    if (_stream.Read(block, 0, blockSize) > 0)
                    {
                        return new Block(_blocksCount++, block);
                    }

                    return null;
                }
            }
            catch
            {
                throw new Exception("Произошла ошибка при чтении файла!");
            }
        }
    }
}
