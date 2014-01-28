using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualObjects.Queries.ConcurrentReader
{
    internal class BlockingDataReader : ConcurrentDataReaderBase
    {
        private readonly ThreadLocal<ITuple> _consumerTuple = new ThreadLocal<ITuple>();
        private readonly ICollection<ITuple> _data = new List<ITuple>();

        private readonly Task _loadDataRows;

        private readonly BlockingCollection<DataRow> _loadedRows = new BlockingCollection<DataRow>();
        private readonly Task _mapIntoTuples;
        private readonly BlockingCollection<ITuple> _transformedRows = new BlockingCollection<ITuple>();
        private IDataReader _reader;

        public BlockingDataReader(IDataReader reader, Predicate<IDataReader> readWhile = null)
        {
            _reader = reader;

            var f = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

            _loadDataRows = f.StartNew(() => LoadingWork(readWhile));
            _mapIntoTuples = f.StartNew(() => MapDataRows());
        }

        private void MapDataRows()
        {
            foreach ( DataRow row in _loadedRows.GetConsumingEnumerable() )
            {
                try
                {
                    var tuple = row.ToTuple(this);
                    _transformedRows.Add(tuple);
                    _data.Add(tuple);
                }
                catch ( Exception )
                {
                    _transformedRows.CompleteAdding();
                    throw;
                }
            }
            _transformedRows.CompleteAdding();
        }

        private void LoadingWork(Predicate<IDataReader> readWhile = null)
        {
            if ( readWhile == null )
            {
                readWhile = r => true;
            }

            var index = 0;
            try
            {
                if ( _reader.Read() )
                {
                    var columns = _reader.GetColumnNames();

                    do
                    {
                        if ( !readWhile(_reader) )
                        {
                            break;
                        }

                        _loadedRows.Add(new DataRow(index++)
                                            {
                                                ColumnNames = columns,
                                                Values = _reader.GetValues()
                                            });
                    }
                    while (_reader.Read());
                }
            }
            catch (Exception)
            {
                _loadedRows.CompleteAdding();
                _reader.Close();

                throw;
            }

            _loadedRows.CompleteAdding();
            _reader.Close();
        }

        public override void Dispose()
        {
            if ( _reader != null )
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        public override void Close()
        {
            Task.WaitAll(_loadDataRows, _mapIntoTuples);
        }

        public override ITuple GetData()
        {
            return _consumerTuple.Value;
        }

        public override bool Read()
        {
            _consumerTuple.Value = _transformedRows.GetConsumingEnumerable().FirstOrDefault();

            return _consumerTuple.Value != null;
        }

        public override IEnumerable<ITuple> GetTuples()
        {
            return _data;
        }
    }
}
