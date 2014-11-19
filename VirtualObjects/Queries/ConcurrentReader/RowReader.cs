using System;
using System.Collections.Generic;

namespace VirtualObjects.Queries.ConcurrentReader
{
    class RowReader : ConcurrentDataReaderBase
    {
        private readonly DataRow _row;
        private readonly DataRowTuple _tuple;

        public RowReader(DataRow row)
        {
            _row = row;
            _tuple = new DataRowTuple(row);
        }

        public override void Dispose()
        {
            
        }

        public override void Close()
        {
            
        }

        public override ITuple GetData()
        {
            return _tuple;
        }

        public override IEnumerable<ITuple> GetTuples()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            return false;
        }
    }
}