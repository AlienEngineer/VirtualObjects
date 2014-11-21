using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace VirtualObjects.Queries.ConcurrentReader
{
    /// <summary>
    /// 
    /// </summary>
    public class ConcurrentDataReader : ConcurrentDataReaderBase
    {
        private readonly IDataReader _reader;
        private readonly List<ITuple> _data = new List<ITuple>();
        private readonly Thread _loaderThread;

        private readonly ConcurrentDictionary<Thread, ITuple> _threadAllocatedData =
            new ConcurrentDictionary<Thread, ITuple>();

        private int _current;
        private int _running;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDataReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="readWhile">The read while.</param>
        public ConcurrentDataReader(IDataReader reader, Predicate<IDataReader> readWhile = null)
        {
            _reader = reader;
            _loaderThread = new Thread(() => LoadingWork(readWhile));

            FieldCount = _reader.FieldCount;
            Depth = _reader.Depth;
        }

        private void LoadingWork(Predicate<IDataReader> readWhile = null)
        {
            if ( readWhile == null )
            {
                readWhile = r => true;
            }

            while ( _reader.Read() )
            {
                if ( !readWhile(_reader) )
                {
                    break;
                }

                var row = new Dictionary<String, Object>();
                for ( var i = 0; i < _reader.FieldCount; i++ )
                {
                    row[_reader.GetName(i).ToLower()] = _reader[i];
                }

                _data.Add(new Tuple(row, this));
            }
            _reader.Close();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">No data found for the current thread.</exception>
        public override ITuple GetData()
        {
            try
            {
                return _threadAllocatedData[Thread.CurrentThread];
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException("No data found for the current thread.", ex);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close()
        {
            while ( _loaderThread.ThreadState == ThreadState.Unstarted )
            {
                Thread.Sleep(0);
            }

            _loaderThread.Join();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public override void Dispose()
        {
            if ( _reader == null )
            {
                return;
            }
            _reader.Close();
            _reader.Dispose();
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Read()
        {
            if ( Thread.VolatileRead(ref _running) != 1 && Interlocked.CompareExchange(ref _running, 1, 0) == 0 )
            {
                _loaderThread.Start();
            }

            while ( _data.Count == Thread.VolatileRead(ref _current) )
            {
                if ( _reader.IsClosed )
                {
                    return false;
                }
                Thread.Sleep(0);
            }

            var index = Interlocked.Increment(ref _current) - 1;

            if ( index >= _data.Count )
            {
                Interlocked.Decrement(ref _current);
                return Read();
            }

            while ( (_threadAllocatedData[Thread.CurrentThread] = _data[index]) == null )
            {
            }
            return true;
        }

        /// <summary>
        /// Gets the tuples.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ITuple> GetTuples()
        {
            return _data;
        }
    }
}
