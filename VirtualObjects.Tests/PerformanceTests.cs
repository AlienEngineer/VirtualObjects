using System;
using System.Threading.Tasks;
using Fasterflect;

namespace VirtualObjects.Tests
{

#if PERFORMANCE
    using NUnit.Framework;
    using Models.Northwind;

    /// <summary>
    /// 
    /// Description
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Performance")]
    public class PerformanceTests : UtilityBelt
    {
        private const string STR_Activador = "Activador";
        private const string STR_Fasterflect = "Fasterflect";
        private const string STR_New = "New";
        private const string STR_EntityProvider = "EntityProvider";
        private const string STR_EntityProxyProvider = "EntityProxyProvider";
        private const string STR_SetFieldFinalValue = "SetFieldFinalValue";
        private const string STR_SetValue = "SetValue";
        private const string STR_HardCoded = "HandCoded";
        private const string STR_Parallel = "Parallel";
        private const string STR_Compiled = "Compiled";

        public class EntitiesCreation
        {
            public float EntityProxyProvider { get; set; }
            public int NumberOfEntities { get; set; }
            public float EntityProvider { get; set; }
            public float New { get; set; }
            public float Fasterflect { get; set; }
            public float Activator { get; set; }
        }

        public class EntitiesMapping
        {
            public float Compiled { get; set; }
            public float HandCoded { get; set; }
            public int NumberOfEntities { get; set; }
            public float Parallel { get; set; }
            public float SetFieldFinalValue { get; set; }
            public float SetValue { get; set; }
        }


        [Test]
        public void Performance_Check_EntityCreation()
        {

            var type = typeof(Suppliers);
            var mapped = Mapper.Map(typeof(Suppliers));
            var createInstance = mapped.EntityFactory;
            var createProxyInstance = mapped.EntityProxyFactory;

            using ( var session = new ExcelSession("Sessions\\Performance.xlsx") )
            {
                int numberOfEntities = 0;
                do
                {
                    numberOfEntities += 10;

                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            createInstance();
                        }
                    }, name: STR_EntityProvider);

                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            createProxyInstance(Session);
                        }
                    }, name: STR_EntityProxyProvider);

                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            new Suppliers();
                        }
                    }, name: STR_New);

                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            type.CreateInstance();
                        }
                    }, name: STR_Fasterflect);

                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            Activator.CreateInstance(type);
                        }
                    }, name: STR_Activador);

                    session.Insert(new EntitiesCreation
                    {
                        NumberOfEntities = numberOfEntities,
                        Activator = (float)Diagnostic.GetMilliseconds(STR_Activador),
                        EntityProvider = (float)Diagnostic.GetMilliseconds(STR_EntityProvider),
                        EntityProxyProvider = (float)Diagnostic.GetMilliseconds(STR_EntityProxyProvider),
                        Fasterflect = (float)Diagnostic.GetMilliseconds(STR_Fasterflect),
                        New = (float)Diagnostic.GetMilliseconds(STR_New)
                    });

                } while ( numberOfEntities < 500 );
            }
        }

        [Test]
        public void Performance_Check_EntityMapping()
        {
            var type = typeof(Suppliers);
            var entityInfo = Mapper.Map(type);

            Object[] tmpData = new Object[] 
            {
                1,
                "Company Name",
                "Contact Name",
                "ContactTitle",
                "Address",
                "City",
                "Region",
                "PostalCode",
                "Country",
                "Phone",
                "Fax",
                "HomePage"
            };

            var reader = new MockReader(tmpData);
                        
            using ( var session = new ExcelSession("Sessions\\Performance.xlsx") )
            {
                int numberOfEntities = 0;
                do
                {
                    numberOfEntities += 10;

                    var suppliers = GetSuppliers(numberOfEntities);
                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            var supplier = suppliers[i];
                            var data = reader.GetValues();
                            for ( int j = 0; j < data.Length; j++ )
                            {
                                entityInfo.Columns[j].SetFieldFinalValue(supplier, data[j]);
                            }
                        }
                    }, name: STR_SetFieldFinalValue);

                    suppliers = GetSuppliers(numberOfEntities);
                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            var supplier = suppliers[i];
                            var data = reader.GetValues();
                            for ( int j = 0; j < data.Length; j++ )
                            {
                                entityInfo.Columns[j].SetValue(supplier, data[j]);
                            }
                        }
                    }, name: STR_SetValue);

                    suppliers = GetSuppliers(numberOfEntities);

                    Diagnostic.Timed(() =>
                    {
                        var sups = suppliers;
                        Parallel.For(0, suppliers.Length, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
                        {

                            var supplier = sups[i];
                            entityInfo.MapEntity(supplier, reader);

                        });
                    }, name: STR_Parallel);

                    suppliers = GetSuppliers(numberOfEntities);
                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            var data = reader.GetValues();
                            var supplier = suppliers[i];

                            supplier.SupplierId = (int)data[0];
                            supplier.CompanyName = (String)data[1];
                            supplier.ContactName = (String)data[2];
                            supplier.ContactTitle = (String)data[3];
                            supplier.Address = (String)data[4];
                            supplier.City = (String)data[5];
                            supplier.Region = (String)data[6];
                            supplier.PostalCode = (String)data[7];
                            supplier.Country = (String)data[8];
                            supplier.Phone = (String)data[9];
                            supplier.Fax = (String)data[10];
                            supplier.HomePage = (String)data[11];
                        }
                    }, name: STR_HardCoded);

                    suppliers = GetSuppliers(numberOfEntities);
                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            var supplier = suppliers[i];
                            entityInfo.MapEntity(supplier, reader);
                        }
                    }, name: STR_Compiled);

                    session.Insert(new EntitiesMapping
                    {
                        NumberOfEntities = numberOfEntities,
                        SetFieldFinalValue = (float)Diagnostic.GetMilliseconds(STR_SetFieldFinalValue),
                        SetValue = (float)Diagnostic.GetMilliseconds(STR_SetValue),
                        HandCoded = (float)Diagnostic.GetMilliseconds(STR_HardCoded),
                        Parallel = (float)Diagnostic.GetMilliseconds(STR_Parallel),
                        Compiled = (float)Diagnostic.GetMilliseconds(STR_Compiled)
                    });

                } while ( numberOfEntities < 500 );
            }

        }

        private static Suppliers[] GetSuppliers(int howMany)
        {
            Suppliers[] result = new Suppliers[howMany];

            for ( int i = 0; i < howMany; i++ )
            {
                result[i] = new Suppliers();
            }

            return result;
        }

    }
#endif
}