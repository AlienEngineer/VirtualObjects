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
            public int Atempt { get; set; }
            public float VirtualObjects { get; set; }
            public float Dapper { get; set; }
            public float Diff { get; set; }
        }

        class CountSuppliers : PerfRecord { }

        [Test]
        public void Performance_With_ExcelRecords()
        {
            const int maxRepeat = 50;
            using ( var session = new ExcelSession("Sessions\\Performance.xlsx") )
            {
                for ( int i = 0; i < maxRepeat; i++ )
                {

                    Diagnostic.Timed(() =>
                        Session.Count<Suppliers>()
                    );

                    var virtualObjects = (float)Diagnostic.GetMilliseconds();

                    Diagnostic.Timed(() => Connection.Query<int>("Select Count(*) from Suppliers"));

                    var dapper = (float)Diagnostic.GetMilliseconds();

                    if ( i > 0 )
                    {
                        session.Insert(new CountSuppliers
                        {
                            Atempt = i,
                            VirtualObjects = virtualObjects,
                            Dapper = dapper,
                            Diff = virtualObjects - dapper
                        });
                    }
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
