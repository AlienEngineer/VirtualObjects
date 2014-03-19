using VirtualObjects;
using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;

public class Internal_Builder_Dynamic_f__AnonymousType2d_2
{

    public static Object MapObject(Object entity, Object[] data)
    {
        return Map(entity, data);
    }

    public static Object Make()
    {
        return new ExpandoObject();
    }

    public static Object MakeProxy(ISession session)
    {
        return new ExpandoObject();
    }

    private static System.Reflection.ConstructorInfo ctor;
    private static System.Reflection.ParameterInfo[] parameters;

    public static void Init(Type type)
    {
        ctor = type.GetConstructors().Single();
        parameters = ctor.GetParameters();
    }

    public static Object EntityCast(Object source)
    {
        IDictionary<string, object> dict = (ExpandoObject)source;

        var parameterValues = parameters.Select(p => dict[p.Name]).ToArray();

        return ctor.Invoke(parameterValues);
    }

    public static Object Map(dynamic entity, Object[] data)
    {
        int i = 0;

        entity.Order = FillEntity_Order(data, out i, i);
        ++i;

        entity.OrderDetails = FillCollection_OrderDetails<VirtualObjects.Tests.Models.Northwind.OrderDetails>(data, out i, i);
        ++i;


        return entity;
    }

    private static Object Parse(Object value)
    {
        if (value == null || value == DBNull.Value)
        {
            return null;
        }

        return value;
    }


    private static VirtualObjects.Tests.Models.Northwind.Orders FillEntity_Order(Object[] data, out int i, int index)
    {
        i = index;
        var entity = new VirtualObjects.Tests.Models.Northwind.Orders();


        entity.OrderId = (Int32)(Parse(data[i]) ?? default(Int32)); ++i;

        entity.Customer = new VirtualObjects.Tests.Models.Northwind.Customers();

        entity.Employee = new VirtualObjects.Tests.Models.Northwind.Employee();

        entity.OrderDate = (DateTime)(Parse(data[i]) ?? default(DateTime)); ++i;

        entity.RequiredDate = (DateTime)(Parse(data[i]) ?? default(DateTime)); ++i;

        entity.ShippedDate = (DateTime)(Parse(data[i]) ?? default(DateTime)); ++i;

        entity.Shipper = new VirtualObjects.Tests.Models.Northwind.Shippers();

        entity.Freight = (Decimal)(Parse(data[i]) ?? default(Decimal)); ++i;

        entity.ShipName = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipAddress = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipCity = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipRegion = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipPostalCode = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipCountry = (String)(Parse(data[i]) ?? default(String)); ++i;


        return entity;
    }
    private static IList<TEntity> FillCollection_OrderDetails<TEntity>(Object[] data, out int i, int index) where TEntity : class, new()
    {
        i = index;
        var list = new List<TEntity>();



        return list;
    }


}
