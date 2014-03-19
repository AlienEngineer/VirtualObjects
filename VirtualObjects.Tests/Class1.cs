using VirtualObjects;
using VirtualObjects.Queries.Mapping;
using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Data;

public class Internal_Builder_Dynamic_f__AnonymousType21_2
{

    public static MapResult MapObject(Object entity, IDataReader reader)
    {
        return Map(entity, reader);
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

    public static MapResult Map(dynamic entity, IDataReader reader)
    {
        int i = 0;
        var data = reader.GetValues();
        var hasMoreData = false;
        
        entity.Order = FillEntity_Order(data, out i, i);

        entity.Details = FillCollection_Details(reader, out i, i, out hasMoreData);

        
        return new MapResult {
            Entity = entity,
            HasMore = hasMoreData
        };
    }

    private static Object Parse(Object value)
    {
        if ( value == null || value == DBNull.Value )
        {
            return null;
        }

        return value;
    }

    
    private static VirtualObjects.Tests.Models.Northwind.Orders FillEntity_Order(Object[] data, out int i, int index)
    {
        i = index;
        var entity = new VirtualObjects.Tests.Models.Northwind.Orders();

        
        entity.OrderId  = (Int32)(Parse(data[i]) ?? default(Int32)); ++i;

        entity.Customer  = new VirtualObjects.Tests.Models.Northwind.Customers(); ++i;

        entity.Employee  = new VirtualObjects.Tests.Models.Northwind.Employee(); ++i;

        entity.OrderDate  = (DateTime)(Parse(data[i]) ?? default(DateTime)); ++i;

        entity.RequiredDate  = (DateTime)(Parse(data[i]) ?? default(DateTime)); ++i;

        entity.ShippedDate  = (DateTime)(Parse(data[i]) ?? default(DateTime)); ++i;

        entity.Shipper  = new VirtualObjects.Tests.Models.Northwind.Shippers(); ++i;

        entity.Freight  = (Decimal)(Parse(data[i]) ?? default(Decimal)); ++i;

        entity.ShipName  = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipAddress  = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipCity  = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipRegion  = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipPostalCode  = (String)(Parse(data[i]) ?? default(String)); ++i;

        entity.ShipCountry  = (String)(Parse(data[i]) ?? default(String)); ++i;


        return entity;
    }
    private static IList<VirtualObjects.Tests.Models.Northwind.OrderDetails> FillCollection_Details(IDataReader reader, out int i, int index, out Boolean hasMoreData)
    {
        
        var list = new List<VirtualObjects.Tests.Models.Northwind.OrderDetails>();
        Object id = null;
        hasMoreData = false;

        do {
            i = index;
            var data = reader.GetValues();
            var entity = new VirtualObjects.Tests.Models.Northwind.OrderDetails();

            
        entity.Order  = new VirtualObjects.Tests.Models.Northwind.Orders(); ++i;

        entity.Product  = new VirtualObjects.Tests.Models.Northwind.Products(); ++i;

        entity.UnitPrice  = (Decimal)(Parse(data[i]) ?? default(Decimal)); ++i;

        entity.Quantity  = (Int16)(Parse(data[i]) ?? default(Int16)); ++i;

        entity.Discount  = (Single)(Parse(data[i]) ?? default(Single)); ++i;


            list.Add(entity);
        } while((id = reader[0]) != null && (hasMoreData = reader.Read()) && id.ToString() == reader[0 + index].ToString());
        
        return list;
    }

   
}
