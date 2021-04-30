using System.IO;

namespace GZipArchiver
{
    public class Writer
    {
        private readonly Stream _stream;

        public Writer(Stream stream)
        {
            _stream = stream;
        }

        public void WriteBlock(Block block)
        {
            _stream.Write(block.Bytes, 0, block.Bytes.Length);
        }
    }
}
