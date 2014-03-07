using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    using NUnit.Framework;
    using Dapper;
    using System;
    using System.Linq;

    /// <summary>
    /// 
    /// Testing VODB vs Dapper
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Performance")]
    public class PerformanceCheck : UtilityBelt
    {

        class PerfRecord
        {
            public String Name { get; set; }
            public int Atempt { get; set; }
            public String Operation { get; set; }
            public Double Milliseconds { get; set; }
        }

        private void RegisterTime(ISession session, String framework, int atempt, String operation, Double time)
        {

            session.Insert(new PerfRecord
            {
                Name = framework,
                Atempt = atempt,
                Operation = operation,
                Milliseconds = time
            });
        }

        [Test]
        public void Performance_With_ExcelRecords()
        {
            using ( var session = new ExcelSession("Sessions\\Performance.xlsx") )
            {
                for ( int i = 0; i < 10; i++ )
                {
                    Diagnostic.Timed(() => Session.Count<Suppliers>());
                    RegisterTime(session, "VirtualObjects", i, "Count Suppliers", Diagnostic.GetMilliseconds());

                    Diagnostic.Timed(() => Connection.Query<int>("Select Count(*) from Suppliers"));
                    RegisterTime(session, "Dapper", i, "Count Suppliers", Diagnostic.GetMilliseconds());
                }
            }
        }

        [Test, Repeat(Repeat)]
        public void Performance_Dapper_GetAll_Suppliers()
        {
            Diagnostic.Timed(() =>
            {
                foreach ( var supplier in Connection.Query<Suppliers>("Select * from Suppliers") )
                {
                    Assert.That(supplier.SupplierId, Is.GreaterThan(0));
                }
            });
        }

        [Test, Repeat(Repeat)]
        public void Performance_VO_GetAll_Suppliers()
        {
            Diagnostic.Timed(() =>
            {
                foreach ( var supplier in Session.GetAll<Suppliers>() )
                {
                    Assert.That(supplier.SupplierId, Is.GreaterThan(0));
                }
            });
        }

        [Test, Repeat(Repeat)]
        public void Performance_Dapper_GetAll_OrderDetails()
        {
            Diagnostic.Timed(() =>
            {
                foreach ( var supplier in Connection.Query<OrderDetailsSimplified>("Select * from [Order Details]") )
                {
                    Assert.That(supplier.OrderId, Is.GreaterThan(0));
                }
            });
        }

        [Test, Repeat(Repeat)]
        public void Performance_VO_GetAll_OrderDetails()
        {
            Diagnostic.Timed(() =>
            {
                foreach ( var supplier in Session.GetAll<OrderDetailsSimplified>() )
                {
                    Assert.That(supplier.OrderId, Is.GreaterThan(0));
                }

            });
        }

        [Test, Repeat(1000)]
        public void Performance_Dapper_GetAll_Supplier_ManyTimes()
        {
            Diagnostic.Timed(() =>
            {
                foreach ( var supplier in Connection.Query<Suppliers>("Select * from Suppliers") )
                {
                    Assert.That(supplier.SupplierId, Is.GreaterThan(0));
                }
            });
        }

        [Test, Repeat(1000)]
        public void Performance_VO_GetAll_Supplier_ManyTimes()
        {
            Diagnostic.Timed(() =>
            {
                foreach ( var supplier in Session.GetAll<Suppliers>() )
                {
                    Assert.That(supplier.SupplierId, Is.GreaterThan(0));
                }
            });
        }

    }
}
