using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Sessions
{
    using NUnit.Framework;
    using Dapper;

    /// <summary>
    /// 
    /// Testing VODB vs Dapper
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Performance")]
    public class PerformanceCheck : UtilityBelt
    {

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
