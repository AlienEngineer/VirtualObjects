using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    using NUnit.Framework;
    using Dapper;
    using System;
    using System.Linq;
    using System.Data.Entity;
using System.Data.Common;

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
            public float EntityFramework { get; set; }
            public float Dapper { get; set; }
            public float HardCoded { get; set; }
        }

        class CountSuppliers : PerfRecord { }

        class EFContext : DbContext
        {
            public EFContext(DbConnection connection) : base(connection, false)
            {
            }

            public DbSet<Suppliers> Suppliers { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                // modelBuilder.Entity<Suppliers>()

                base.OnModelCreating(modelBuilder);
            }
        }

        [Test]
        public void Performance_With_ExcelRecords()
        {
            Connection.Open();

            var ef = new EFContext((DbConnection)Connection);

            const int maxRepeat = 50;
            using ( var session = new ExcelSession("Sessions\\Performance.xlsx") )
            {
                for ( int i = 0; i < maxRepeat; i++ )
                {
                    //
                    // Dapper
                    //        
                    Diagnostic.Timed(() => Connection.Query<int>("Select Count(*) from Suppliers"));

                    var dapper = (float)Diagnostic.GetMilliseconds();

                    //
                    // Entity Framework
                    //
                    Diagnostic.Timed(() => ef.Suppliers.Count());
                    
                    var entityFramework = (float)Diagnostic.GetMilliseconds();

                    //
                    // HardCoded
                    //
                    Diagnostic.Timed(() =>
                    {
                        var cmd = Connection.CreateCommand();
                        cmd.CommandText = "Select Count(*) from Suppliers";
                        cmd.ExecuteScalar();
                    });

                    var hardCoded = (float)Diagnostic.GetMilliseconds();

                    //
                    // VirtualObjects
                    //
                    Diagnostic.Timed(() => Session.Count<Suppliers>());

                    var virtualObjects = (float)Diagnostic.GetMilliseconds();

                    session.Insert(new CountSuppliers
                    {
                        Atempt = i,
                        EntityFramework = entityFramework,
                        VirtualObjects = virtualObjects,
                        Dapper = dapper,
                        HardCoded = hardCoded
                    });
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
