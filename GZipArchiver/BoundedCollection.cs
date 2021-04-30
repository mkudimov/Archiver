using System.Collections.Generic;
using System.Threading;

namespace GZipArchiver
{
    public class BoundedCollection<T> where T : class
    {
        private readonly List<T> _collection = new List<T>();
        private readonly int _capacity = -1;

        public BoundedCollection() { }
        public BoundedCollection(int capacity) { _capacity = capacity; }

        public void Add(T item)
        {
            _collection.Add(item);
        }

        public T Find(T item)
        {
            T result = _collection.Find(x => x.Equals(item));
            _collection.Remove(result);
            return result;
        }

        public bool Contains(T item)
        {
            return _collection.Exists(x => x.Equals(item));
        }

        public int FreePositions
        {
            get
            {
                if (_capacity == -1)
                    return -1;
                else
                    return _capacity - _collection.Count;
            }
        }
    }
}
