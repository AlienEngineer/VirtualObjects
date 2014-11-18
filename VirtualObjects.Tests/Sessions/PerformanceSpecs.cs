using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Dapper;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{

    

    [Tags("Performance")]
    public abstract class PerformanceSpecs : SpecsUtilityBelt
    {
        #region CONTEXT

        public class EFContext : DbContext
        {
            public EFContext(DbConnection connection)
                : base(connection, false)
            {
            }

            public DbSet<Suppliers> Suppliers { get; set; }
            public DbSet<OrderDetails> OrderDetails { get; set; }

        }

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

        [Mappings.Table(TableName = "Order Details")]
        [Table("Order Details")]
        public class OrderDetails
        {
            [Key]
            public int OrderId { get; set; }

            public int ProductId { get; set; }

            public Decimal UnitPrice { get; set; }

            public Int16 Quantity { get; set; }

            public Single Discount { get; set; }
        }

        #endregion

        protected const string STR_Dapper = "Dapper";
        protected const string STR_EntityFramework = "EntityFramework";
        protected const string STR_VirtualObjects = "VirtualObjects";
        protected const string STR_HardCoded = "HandCoded";
        protected const int NUMBER_OF_EXECUTIONS = 500;
        protected const int EXECUTION_STEP = 10;

        Establish context = () =>
        {

            EntityFramework = new EFContext((DbConnection)Connection);
            VObjects = new Session(connectionName: "Northwind");

        };

        protected static EFContext EntityFramework;
        protected static Session VObjects;

        private static string GetValue(IDataReader reader, String fieldName)
        {
            var value = reader[fieldName];

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return (String)value;
        }

        protected static IEnumerable<Suppliers> MapSupplier(IDataReader reader)
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

        protected static IEnumerable<dynamic> MapDynamicSupplier(IDataReader reader)
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

        protected static IEnumerable<OrderDetails> MapOrderDetail(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return new OrderDetails
                {
                    OrderId = (int)reader["OrderId"],
                    Discount = (float)reader["Discount"],
                    ProductId = (int)reader["ProductId"],
                    Quantity = (short)reader["Quantity"],
                    UnitPrice = (decimal)reader["UnitPrice"]
                };
            }
        }
    }

    [Subject("PerformanceSpecs")]
    public class When_mapping_suppliers : PerformanceSpecs
    {


        private Because of = () =>
        {

            int numberOfExecutions = 0;
            do
            {
                numberOfExecutions += EXECUTION_STEP;

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
                        EntityFramework.Suppliers.ToList();
                    }
                }, name: STR_EntityFramework);

                Diagnostic.Timed(() =>
                {
                    VObjects.KeepAlive(() =>
                    {
                        for (int i = 0; i < numberOfExecutions; i++)
                        {
                            VObjects.GetAll<Suppliers>().ToList();
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

                _results.Add(new MappingSuppliers
                {
                    NumberOfExecutions = numberOfExecutions,
                    EntityFramework = (float)Diagnostic.GetMilliseconds(STR_EntityFramework),
                    VirtualObjects = (float)Diagnostic.GetMilliseconds(STR_VirtualObjects),
                    Dapper = (float)Diagnostic.GetMilliseconds(STR_Dapper),
                    HandCoded = (float)Diagnostic.GetMilliseconds(STR_HardCoded)
                });

            } while (numberOfExecutions < NUMBER_OF_EXECUTIONS);

        };


        private It should_allways_have_virtual_objects_as_lower_result =
            () =>
            {
                //
                // Allow results to be worst in 2 iterations.
                // At times a thread could be blocked and therefore vo could be slower in those cases. 
                int failTolerance = 2;

                int i = 0;
                
                foreach (var result in _results)
                {
                    try
                    {
                        result.VirtualObjects.Should().BeLessOrEqualTo(result.Dapper, "VirtualObjects should be faster than Dapper on the {0} iteration", i);
                        result.VirtualObjects.Should().BeLessOrEqualTo(result.EntityFramework, "VirtualObjects should be faster than EntityFramework on the {0} iteration", i);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" => " + ex.Message);
                        --failTolerance;

                        if (failTolerance == 0)
                        {
                            throw;
                        }
                    }
                    
                    ++i;
                }
            };

        private static IList<MappingSuppliers> _results = new List<MappingSuppliers>();
    }
}
