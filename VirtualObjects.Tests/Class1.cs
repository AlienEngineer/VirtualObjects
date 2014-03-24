using VirtualObjects;
using VirtualObjects.Queries.Mapping;
using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Data;

public class Internal_Builder_Dynamic_f__AnonymousType1d_5
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

        try
        {
            //if (data[0] != DBNull.Value)
            entity.OrderId = (Int32)(Parse(data[i]) ?? default(Int32));
        }
        catch ( InvalidCastException )
        {
            entity.OrderId = (Int32)Convert.ChangeType((Parse(data[i]) ?? default(Int32)), typeof(Int32));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [OrderId] with [" + data[0] + "] value.", ex);
        }
        ++i;

        try
        {
            //if (data[1] != DBNull.Value)
            entity.UnitPrice = (Decimal)(Parse(data[i]) ?? default(Decimal));
        }
        catch ( InvalidCastException )
        {
            entity.UnitPrice = (Decimal)Convert.ChangeType((Parse(data[i]) ?? default(Decimal)), typeof(Decimal));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [UnitPrice] with [" + data[1] + "] value.", ex);
        }
        ++i;

        try
        {
            //if (data[2] != DBNull.Value)
            entity.Quantity = (Int16)(Parse(data[i]) ?? default(Int16));
        }
        catch ( InvalidCastException )
        {
            entity.Quantity = (Int16)Convert.ChangeType((Parse(data[i]) ?? default(Int16)), typeof(Int16));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [Quantity] with [" + data[2] + "] value.", ex);
        }
        ++i;

        try
        {
            //if (data[3] != DBNull.Value)
            entity.ShipName = Parse(data[i]);
        }
        catch ( InvalidCastException )
        {
            entity.ShipName = Convert.ChangeType((Parse(data[i])), typeof(String));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [ShipName] with [" + data[3] + "] value.", ex);
        }
        ++i;

        try
        {
            //if (data[4] != DBNull.Value)
            entity.Employee = Parse(data[i]);
        }
        catch ( InvalidCastException )
        {
            entity.Employee = Convert.ChangeType((Parse(data[i])), typeof(String));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [Employee] with [" + data[4] + "] value.", ex);
        }
        ++i;


        return new MapResult
        {
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




}