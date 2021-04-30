using System.IO;

namespace GZipArchiver
{
    public abstract class BlockReader
    {
        protected Stream _stream;
        protected long _streamLength;
        protected int _blocksCount;
        protected object _locker = new object();
        public abstract Block ReadBlock();

        protected BlockReader(Stream stream)
        {
            _stream = stream;
            _streamLength = _stream.Length;
        }


        #region Getters and setters
        public long StreamPosition
        {
            get
            {
                lock(_locker)
                {
                    return _stream.Position;
                }
            }
        }

        public long StreamLength
        {
            get
            {
                return _streamLength;
            }
        }
        #endregion
    }
}
