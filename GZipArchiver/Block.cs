namespace GZipArchiver
{
    public class Block
    {
        private readonly int _index;
        private byte[] _bytes;

        public Block(int index, byte[] bytes)
        {
            _index = index;
            _bytes = bytes;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Block;

            if (item == null)
            {
                return false;
            }

            return Index == item.Index;
        }

        public override int GetHashCode()
        {
            return _bytes.GetHashCode();
        }

        #region Getters and setters
        public int Index
        {
            get { return _index; }
        }

        public byte[] Bytes
        {
            get { return _bytes; }
        }
        #endregion
    }
}
