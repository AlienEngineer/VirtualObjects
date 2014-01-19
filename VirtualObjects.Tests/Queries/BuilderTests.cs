using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using FluentAssertions;
using VirtualObjects.Queries;

namespace VirtualObjects.Tests.Queries
{
    using NUnit.Framework;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.SqlClient;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// Description
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture]
    public class BuilderTests
    {
        [Table]
        public class Employees
        {
            [Column]
            public int EmployeeId { get; set; }
        }

        readonly IDbConnection connection;

        public BuilderTests()
        {
            connection = new SqlConnection(@"
                    Data Source=(LocalDB)\v11.0;
                    AttachDbFilename=" + Environment.CurrentDirectory + @"\Data\northwnd.mdf;
                    Integrated Security=True;
                    Connect Timeout=30");
        }

        [Test]
        public void TestName()
        {
            IConverter converter = new Converter(connection, typeof(System.Data.Linq.SqlClient.SqlProvider));

            var employees = new List<Employees>().AsQueryable().Where(e => e.EmployeeId == 1);

            var cmd = converter.ConvertToQuery(employees).Command;

            var model = new AttributeMappingSource().GetModel(typeof (Employees));

            model.ContextType.Should().Be<Employees>();
            var datamembers = model.GetMetaType(typeof (Employees)).DataMembers;

            cmd.Should().NotBeNull();
            cmd.CommandText.Should().Be((""));
        }

    }
  
}
