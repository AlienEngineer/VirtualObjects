using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace VirtualObjects.Tests
{
    using NUnit.Framework;
    using VirtualObjects.Tests.Models.Northwind;
    using VirtualObjects.Mappings;

    /// <summary>
    /// 
    /// Description
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture]
    public class PerformanceTests : UtilityBelt
    {
        private const string STR_Activador = "Activador";
        private const string STR_Fasterflect = "Fasterflect";
        private const string STR_New = "New";
        private const string STR_EntityProvider = "EntityProvider";

        class PerfRecord
        {
            public int NumberOfEntities { get; set; }
            public float EntityProvider { get; set; }
            public float New { get; set; }
            public float Fasterflect { get; set; }
            public float Activator { get; set; }
        }

        [Table(TableName = "Factory")]
        class EntitiesCreation : PerfRecord { }            

        [Test]
        public void EntityCreation_Performance_Check()
        {
            var provider = Make<IEntityProvider>();
            var type = typeof(Suppliers);

            using ( var session = new ExcelSession("Performance.xlsx") )
            {
                int numberOfEntities = 0;
                do
                {
                    numberOfEntities += 10;

                    var ep = provider.GetProviderForType(type);

                    Diagnostic.Timed(() =>
                    {
                        for ( int i = 0; i < numberOfEntities; i++ )
                        {
                            ep.CreateEntity(type);
                        }
                    }, name: STR_EntityProvider);                        
                    

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
                        Fasterflect = (float) Diagnostic.GetMilliseconds(STR_Fasterflect),
                        New = (float)Diagnostic.GetMilliseconds(STR_New)
                    });

                } while (numberOfEntities < 500);   
            }
        }
    }
}
