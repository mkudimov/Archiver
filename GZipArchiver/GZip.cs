using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GZipArchiver
{
    public abstract class GZip
    {
        protected int _threadsCount = Environment.ProcessorCount;
        protected BoundedCollection<Block> _collectionForWrite;
        protected string _inputPath;
        protected string _outputPath;
        protected bool _isFinish = false;
        protected Action<string> _eventHandler;
        protected List<Thread> _threads;
        protected Thread _writer;
        protected BlockReader _reader;
        protected ProcessingResult _resultProcess = ProcessingResult.OK;

        private int _IndexNextBlock = 0;
        private object _locker = new object();
        protected GZip(string inputPath)
        {
            _collectionForWrite = new BoundedCollection<Block>(_threadsCount + 2);
            _inputPath = inputPath;
            _threads = new List<Thread>();
        }

        protected GZip(string inputPath, string outputPath) : this(inputPath)
        {
            _outputPath = outputPath;
        }

        public void Start()
        {
            try
            {
                var inputFileInfo = new FileInfo(_inputPath);
                using (var inputFileStream = inputFileInfo.OpenRead())
                {
                    _reader = CreateReader(inputFileStream);
                    _writer = new Thread(Write);
                    _writer.Start();
                    for (int i = 0; i < _threadsCount; i++)
                    {
                        var thread = new Thread(ReadAndProcess);
                        thread.Start();
                        _threads.Add(thread);
                    }

                    _threads?.ForEach(th => th.Join());
                    ReadFinish();
                    _writer?.Join();
                }
            }
            catch
            {
                new Thread(() => { _eventHandler.Invoke("Произошла ошибка при запуске процесса!"); }).Start();
            }
        }
        protected void ReadAndProcess()
        {
            try
            {
                Block block;
                while (((block = _reader.ReadBlock()) != null) && !_isFinish)
                {
                    AddBlockForWrite(new Block(block.Index, Process(block.Bytes)));
                    int percent = (int)(((double)_reader.StreamPosition / _reader.StreamLength) * 100);
                    _eventHandler.Invoke(string.Format("{0}", percent));
                }
            }
            catch (Exception ex)
            {
                new Thread(() => { _eventHandler.Invoke(ex.Message); }).Start();
            }
        }
        protected void Write()
        {
            try
            {
                using (var stream = new FileStream(_outputPath, FileMode.OpenOrCreate))
                {
                    var writer = new Writer(stream);
                    Block block;
                    while ((block = TakeFromCollectionForWrite()) != null)
                    {
                        writer.WriteBlock(block);
                    }
                }
            }
            catch 
            {
                new Thread(() => { _eventHandler.Invoke("Произошла ошибка при записи в файл!"); }).Start();
            }
        }

        public void Stop()
        {
            if (!_isFinish)
            {
                ReadFinish();
                _resultProcess = ProcessingResult.Error;
                _threads?.ForEach(th => th.Join());
                _writer?.Join();
                File.Delete(_outputPath);
            }
        }

        public void OnEventHandler(Action<string> handler)
        {
            _eventHandler += handler;
        }
        
        private bool CanAdd(Block block)
        {
            if ((block.Index == _IndexNextBlock)
                || (_collectionForWrite.FreePositions > 1)
                || (_collectionForWrite.Contains(new Block(_IndexNextBlock, null)) && _collectionForWrite.FreePositions == 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void AddBlockForWrite(Block block)
        {
            lock (_locker)
            {
                while (!CanAdd(block))
                {
                    Monitor.Wait(_locker);
                }
                _collectionForWrite.Add(block);
                Monitor.PulseAll(_locker);
            }
        }
        private Block TakeFromCollectionForWrite()
        {
            lock (_locker)
            {
                Block block;
                while ((block = _collectionForWrite.Find(new Block(_IndexNextBlock, null))) == null
                    && !_isFinish)
                {
                    Monitor.Wait(_locker);
                }
                _IndexNextBlock++;
                Monitor.PulseAll(_locker);
                return block;
            }
        }
        private void ReadFinish()
        {
            lock (_locker)
            {
                _isFinish = true;
                Monitor.PulseAll(_locker);
            }
        }
        protected abstract byte[] Process(byte[] blockBytes);
        protected abstract BlockReader CreateReader(Stream stream);
        
        #region Getters and setters
        public ProcessingResult ResultProcess
        {
            get
            {
                return _resultProcess;
            }
        }
        #endregion

    }
}
