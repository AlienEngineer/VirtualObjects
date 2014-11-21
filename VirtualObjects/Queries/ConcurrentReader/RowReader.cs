using System;
using System.Collections.Generic;

namespace VirtualObjects.Queries.ConcurrentReader
{
    class RowReader : ConcurrentDataReaderBase
    {
        private readonly DataRow _row;
        private readonly DataRowTuple _tuple;

        /// <summary>
        /// Initializes a new instance of the <see cref="RowReader"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public RowReader(DataRow row)
        {
            _row = row;
            _tuple = new DataRowTuple(row);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            
        }

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader" /> Object.
        /// </summary>
        public override void Close()
        {
            
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        public override ITuple GetData()
        {
            return _tuple;
        }

        /// <summary>
        /// Gets the tuples.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IEnumerable<ITuple> GetTuples()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public override bool Read()
        {
            return false;
        }
    }
}