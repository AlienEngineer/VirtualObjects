using System.ComponentModel.DataAnnotations.Schema;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
#if PERFORMANCE
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
        private const string STR_Dapper = "Dapper";
        private const string STR_EntityFramework = "EntityFramework";
        private const string STR_VirtualObjects = "VirtualObjects";
        private const string STR_HardCoded = "HandCoded";

        public class PerfRecord
        {
            public int NumberOfExecutions { get; set; }
            public float VirtualObjects { get; set; }
            public float EntityFramework { get; set; }
            public float Dapper { get; set; }
            public float HandCoded { get; set; }
        }

        public class CountSuppliers : PerfRecord { }

        public class MappingSuppliers : PerfRecord { }

        public class MappingOrderDetails : PerfRecord { }
        
        public class MappingDynamicSuppliers : PerfRecord { }
                  
        [VirtualObjects.Mappings.Table(TableName = "Order Details")]
        [Table("Order Details")]
        public class OrderDetails1
        {
            [System.ComponentModel.DataAnnotations.Key]
            public int OrderId { get; set; }

            public int ProductId { get; set; }

            public Decimal UnitPrice { get; set; }

            public Int16 Quantity { get; set; }

            public Single Discount { get; set; }
        }

        class EFContext : DbContext
        {
            public EFContext(DbConnection connection)
                : base(connection, false)
            {
            }
            
            public DbSet<Suppliers> Suppliers { get; set; }
            public DbSet<OrderDetails1> OrderDetails { get; set; }

        }

        private static string GetValue(IDataReader reader, String fieldName)
        {
            var value = reader[fieldName];

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return (String)value;
        }

        private static IEnumerable<Suppliers> MapSupplier(System.Data.IDataReader reader)
        {
            while (reader.Read())
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

        private static IEnumerable<dynamic> MapDynamicSupplier(System.Data.IDataReader reader)
        {
            while (reader.Read())
            {
                yield return new
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

        private static IEnumerable<OrderDetails1> MapOrderDetail(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return new OrderDetails1
                {
                    OrderId = (int)reader["OrderId"],
                    Discount = (float)reader["Discount"],
                    ProductId = (int)reader["ProductId"],
                    Quantity = (short)reader["Quantity"],
                    UnitPrice = (decimal)reader["UnitPrice"]
                };
            }
        }

        [Test]
        public void Performance_Check_SuppliersMapping()
        {
            var ef = new EFContext((DbConnection)Connection);

            using (var session = new ExcelSession("Sessions\\Performance.xlsx"))
            {
                int numberOfExecutions = 0;
                do
                {
                    numberOfExecutions += 10;

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            Connection.Query<Suppliers>("Select * from Suppliers").ToList();
                        }
                    }, name: STR_Dapper);

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            ef.Suppliers.ToList();
                        }
                    }, name: STR_EntityFramework);

                    Diagnostic.Timed(() =>
                    {
                        session.KeepAlive(() =>
                        {
                            for (int i = 0; i < numberOfExecutions; i++)
                            {
                                Session.GetAll<Suppliers>().ToList();
                            }
                        });
                    }, name: STR_VirtualObjects);

                    Diagnostic.Timed(() =>
                    {
                        if (Connection.State != ConnectionState.Open)
                            Connection.Open();

                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            var cmd = Connection.CreateCommand();
                            cmd.CommandText = "Select * from Suppliers";
                            var reader = cmd.ExecuteReader();

                            MapSupplier(reader).ToList();

                            reader.Close();
                        }
                        Connection.Close();
                    }, name: STR_HardCoded);

                    session.Insert(new MappingSuppliers
                    {
                        NumberOfExecutions = numberOfExecutions,
                        EntityFramework = (float)Diagnostic.GetMilliseconds(STR_EntityFramework),
                        VirtualObjects = (float)Diagnostic.GetMilliseconds(STR_VirtualObjects),
                        Dapper = (float)Diagnostic.GetMilliseconds(STR_Dapper),
                        HandCoded = (float)Diagnostic.GetMilliseconds(STR_HardCoded)
                    });

                } while (numberOfExecutions < 500);
            }
        }

        [Test]
        public void Performance_Check_DynamicSuppliersMapping()
        {
            var ef = new EFContext((DbConnection)Connection);

            using (var session = new ExcelSession("Sessions\\Performance.xlsx"))
            {
                int numberOfExecutions = 0;
                do
                {
                    numberOfExecutions += 10;

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            Connection.Query("Select * from Suppliers").Select(e => new
                            {
                                e.Address,
                                e.City,
                                e.CompanyName,
                                e.ContactName,
                                e.ContactTitle,
                                e.Country,
                                e.Fax,
                                e.HomePage,
                                e.Phone,
                                e.PostalCode,
                                e.Region,
                                e.SupplierId
                            }).ToList();
                        }
                    }, name: STR_Dapper);

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            ef.Suppliers.Select(e => new { 
                                e.Address,
                                e.City,
                                e.CompanyName,
                                e.ContactName,
                                e.ContactTitle,
                                e.Country,
                                e.Fax,
                                e.HomePage,
                                e.Phone,
                                e.PostalCode,
                                e.Region,
                                e.SupplierId
                            }).ToList();
                        }
                    }, name: STR_EntityFramework);

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            Session.KeepAlive(() =>
                            {
                                Session.GetAll<Suppliers>().Select(e => new
                                {
                                    e.Address,
                                    e.City,
                                    e.CompanyName,
                                    e.ContactName,
                                    e.ContactTitle,
                                    e.Country,
                                    e.Fax,
                                    e.HomePage,
                                    e.Phone,
                                    e.PostalCode,
                                    e.Region,
                                    e.SupplierId
                                }).ToList();
                            });
                        }
                    }, name: STR_VirtualObjects);

                    Diagnostic.Timed(() =>
                    {
                        if (Connection.State != ConnectionState.Open)
                            Connection.Open();

                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            var cmd = Connection.CreateCommand();
                            cmd.CommandText = "Select * from Suppliers";
                            var reader = cmd.ExecuteReader();

                            MapDynamicSupplier(reader).ToList();

                            reader.Close();
                        }
                        Connection.Close();
                    }, name: STR_HardCoded);

                    session.Insert(new MappingDynamicSuppliers
                    {
                        NumberOfExecutions = numberOfExecutions,
                        EntityFramework = (float)Diagnostic.GetMilliseconds(STR_EntityFramework),
                        VirtualObjects = (float)Diagnostic.GetMilliseconds(STR_VirtualObjects),
                        Dapper = (float)Diagnostic.GetMilliseconds(STR_Dapper),
                        HandCoded = (float)Diagnostic.GetMilliseconds(STR_HardCoded)
                    });

                } while (numberOfExecutions < 500);
            }
        }

        [Test]
        public void Performance_Check_OrderDetailsMapping()
        {
            var ef = new EFContext((DbConnection)Connection);

            using (var session = new ExcelSession("Sessions\\Performance.xlsx"))
            {
                int numberOfExecutions = 0;
                do
                {
                    numberOfExecutions += 10;

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            Connection.Query<OrderDetails1>("Select * from [Order Details]").ToList();
                        }
                    }, name: STR_Dapper);

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            ef.OrderDetails.ToList();
                        }
                    }, name: STR_EntityFramework);
                
                    Diagnostic.Timed(() =>
                    {
                        Session.KeepAlive(() =>
                        {
                            for (int i = 0; i < numberOfExecutions; i++)
                            {
                                Session.GetAll<OrderDetails1>().ToList();
                            }
                        });
                    }, name: STR_VirtualObjects);

                    Diagnostic.Timed(() =>
                    {
                        if (Connection.State != ConnectionState.Open)
                            Connection.Open();

                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            var cmd = Connection.CreateCommand();
                            cmd.CommandText = "Select * from [Order Details]";
                            var reader = cmd.ExecuteReader();

                            MapOrderDetail(reader).ToList();

                            reader.Close();
                        }
                        Connection.Close();
                    }, name: STR_HardCoded);

                    session.Insert(new MappingOrderDetails
                    {
                        NumberOfExecutions = numberOfExecutions,
                        EntityFramework = (float)Diagnostic.GetMilliseconds(STR_EntityFramework),
                        VirtualObjects = (float)Diagnostic.GetMilliseconds(STR_VirtualObjects),
                        Dapper = (float)Diagnostic.GetMilliseconds(STR_Dapper),
                        HandCoded = (float)Diagnostic.GetMilliseconds(STR_HardCoded)
                    });

                } while (numberOfExecutions < 500);
            }
        }

        [Test]
        public void Performance_Check_Count()
        {
            var ef = new EFContext((DbConnection)Connection);

            using (var session = new ExcelSession("Sessions\\Performance.xlsx"))
            {
                int numberOfExecutions = 0;
                do
                {
                    numberOfExecutions += 10;

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            Connection.Query<int>("Select Count(*) from Suppliers");
                        }
                    }, name: STR_Dapper);

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            ef.Suppliers.Count();
                        }
                    }, name: STR_EntityFramework);

                    Diagnostic.Timed(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            Session.Count<Suppliers>();
                        }
                    }, name: STR_VirtualObjects);

                    Diagnostic.Timed(() =>
                    {
                        if (Connection.State != ConnectionState.Open)
                            Connection.Open();

                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            var cmd = Connection.CreateCommand();
                            cmd.CommandText = "Select Count(*) from Suppliers";
                            cmd.ExecuteScalar();
                        }
                        Connection.Close();
                    }, name: STR_HardCoded);

                    session.Insert(new CountSuppliers
                    {
                        NumberOfExecutions = numberOfExecutions,
                        EntityFramework = (float)Diagnostic.GetMilliseconds(STR_EntityFramework),
                        VirtualObjects = (float)Diagnostic.GetMilliseconds(STR_VirtualObjects),
                        Dapper = (float)Diagnostic.GetMilliseconds(STR_Dapper),
                        HandCoded = (float)Diagnostic.GetMilliseconds(STR_HardCoded)
                    });

                } while (numberOfExecutions < 500);



            }
        }
    }
#endif
}
