using System;
using System.IO;
using System.Linq;

namespace GZipArchiver
{
    public class DecompressBlockReader : BlockReader
    {
        public DecompressBlockReader(Stream stream) : base(stream) { }
        public override Block ReadBlock()
        {
            try
            {
                lock (_locker)
                {
                    var blockHeader = new byte[8];
                    if (_stream.Read(blockHeader, 0, blockHeader.Length) > 0)
                    {
                        var blockLength = BitConverter.ToInt32(blockHeader, blockHeader.Length - 4);
                        var blockData = new byte[blockLength - 8];
                        if (_stream.Read(blockData, 0, blockData.Length) > 0)
                        {
                            return new Block(_blocksCount++, blockHeader.Concat(blockData).ToArray());
                        }
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
