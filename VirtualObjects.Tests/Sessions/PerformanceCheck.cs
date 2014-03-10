using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    using NUnit.Framework;
    using Dapper;
    using System;
    using System.Linq;
    using System.Data.Entity;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// 
    /// Testing VirualObjects vs Dapper vs EntityFramework and HardCoded.
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

        class MappingSuppliers : PerfRecord { }

        class EFContext : DbContext
        {
            public EFContext(DbConnection connection)
                : base(connection, false)
            {
            }

            public DbSet<Suppliers> Suppliers { get; set; }
        }

        private static string GetValue(IDataReader reader, String fieldName)
        {
            var value = reader["Address"];
            
            if ( value == null || value == DBNull.Value )
            {
                return null;
            }

            return (String)value;
        }
        private static IEnumerable<Suppliers> MapSupplier(System.Data.IDataReader reader)
        {
            while ( reader.Read() )
            {
                yield return new Suppliers
                {
                    SupplierId = (int)reader["SupplierId"],
                    Address = GetValue(reader, "Address"),
                    City = GetValue(reader, "City"),
                    CompanyName = GetValue(reader, "CompanyName"),
                    ContactName = GetValue(reader, "ContactName"),
                    ContactTitle = GetValue(reader, "ContactTitle"),
                    Country = GetValue(reader, "Country"),
                    Fax = GetValue(reader, "Fax"),
                    HomePage = GetValue(reader, "HomePage"),
                    Phone = GetValue(reader, "Phone"),
                    PostalCode = GetValue(reader, "PostalCode"),
                    Region = GetValue(reader, "Region")
                };
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

                //
                // Count Suppliers
                //
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


                //
                // Mapping Suppliers
                //
                for ( int i = 0; i < maxRepeat; i++ )
                {
                    //
                    // Dapper
                    //        
                    Diagnostic.Timed(() => Connection.Query<Suppliers>("Select * from Suppliers").ToList());

                    var dapper = (float)Diagnostic.GetMilliseconds();

                    //
                    // Entity Framework
                    //
                    var suppliers = Diagnostic.Timed(() => ef.Suppliers.ToList());

                    var entityFramework = (float)Diagnostic.GetMilliseconds();

                    //
                    // HardCoded
                    //
                    Diagnostic.Timed(() =>
                    {
                        var cmd = Connection.CreateCommand();
                        cmd.CommandText = "Select * from Suppliers";
                        var reader = cmd.ExecuteReader();

                        MapSupplier(reader).ToList();

                        reader.Close();

                    });

                    var hardCoded = (float)Diagnostic.GetMilliseconds();

                    //
                    // VirtualObjects
                    //
                    Diagnostic.Timed(() => Session.GetAll<Suppliers>().ToList());

                    var virtualObjects = (float)Diagnostic.GetMilliseconds();

                    session.Insert(new MappingSuppliers
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

        //[Test, Repeat(Repeat)]
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

        //[Test, Repeat(Repeat)]
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

        //[Test, Repeat(Repeat)]
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

        //[Test, Repeat(Repeat)]
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

        //[Test, Repeat(1000)]
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

        //[Test, Repeat(1000)]
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
