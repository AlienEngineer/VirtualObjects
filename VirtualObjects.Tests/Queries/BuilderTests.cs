using System;
using System.Linq;

namespace VirtualObjects.Tests.Queries
{
    using NUnit.Framework;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.SqlClient;

    /// <summary>
    /// 
    /// Description
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture]
    public class BuilderTests
    {
        class Northwind : DataContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Northwind" /> class.
            /// </summary>
            public Northwind() 
                : base("data\northwnd.mdf") { }

        }

        /// <summary>
        /// 
        /// Description
        /// 
        /// </summary>
        [Test]
        public void TestName()
        {
            var nw = new Northwind();
            
            nw.GetTable<Employees>();
        }
    }
  
    [Table]
    public class Employees
    {
        public int EmployeeId { get; set; }
    }
}
