using VirtualObjects.Scaffold;
using VirtualObjects;
using VirtualObjects.Queries.Mapping;
using System;
using System.Linq;
using System.Data;

public class Internal_Builder_ForeingKey
{

    public static MapResult MapObject(Object entity, IDataReader reader)
    {
        return Map((VirtualObjects.Scaffold.VirtualObjectsHelper.ForeingKey)entity, reader);
    }

    public static Object Make()
    {
        return new VirtualObjects.Scaffold.VirtualObjectsHelper.ForeingKey();
    }

    public static VirtualObjects.Scaffold.VirtualObjectsHelper.ForeingKey MakeProxy(ISession session)
    {
        return new ForeingKeyProxy(session);
    }

    public class ForeingKeyProxy : VirtualObjects.Scaffold.VirtualObjectsHelper.ForeingKey
    {
        private ISession Session { get; set; }

        public ForeingKeyProxy(ISession session)
        {
            Session = session;
        }


        VirtualObjects.Scaffold.VirtualObjectsHelper.Table _Table;
        Boolean _TableLoaded;

        public override VirtualObjects.Scaffold.VirtualObjectsHelper.Table Table
        {
            get
            {
                if ( !_TableLoaded )
                {

                    _Table = Session.GetById(_Table);
                    _TableLoaded = _Table != null;
                }

                return _Table;
            }
            set
            {
                _Table = value;
            }
        }

        VirtualObjects.Scaffold.VirtualObjectsHelper.Column _Column;
        Boolean _ColumnLoaded;

        public override VirtualObjects.Scaffold.VirtualObjectsHelper.Column Column
        {
            get
            {
                if ( !_ColumnLoaded )
                {
                    _Column.Table = this.Table;
                    _Column = Session.GetById(_Column);
                    _ColumnLoaded = _Column != null;
                }

                return _Column;
            }
            set
            {
                _Column = value;
            }
        }

        VirtualObjects.Scaffold.VirtualObjectsHelper.Column _ReferencedColumn;
        Boolean _ReferencedColumnLoaded;

        public override VirtualObjects.Scaffold.VirtualObjectsHelper.Column ReferencedColumn
        {
            get
            {
                if ( !_ReferencedColumnLoaded )
                {
                    _ReferencedColumn.Table = _ReferencedTable;
                    _ReferencedColumn = Session.GetById(_ReferencedColumn);
                    _ReferencedColumnLoaded = _ReferencedColumn != null;
                }

                return _ReferencedColumn;
            }
            set
            {
                _ReferencedColumn = value;
            }
        }

        VirtualObjects.Scaffold.VirtualObjectsHelper.Table _ReferencedTable;
        Boolean _ReferencedTableLoaded;

        public override VirtualObjects.Scaffold.VirtualObjectsHelper.Table ReferencedTable
        {
            get
            {
                if ( !_ReferencedTableLoaded )
                {

                    _ReferencedTable = Session.GetById(_ReferencedTable);
                    _ReferencedTableLoaded = _ReferencedTable != null;
                }

                return _ReferencedTable;
            }
            set
            {
                _ReferencedTable = value;
            }
        }

    }

    public static void Init(Type type)
    {
    }

    public static MapResult Map(VirtualObjects.Scaffold.VirtualObjectsHelper.ForeingKey entity, IDataReader reader)
    {
        var data = reader.GetValues();


        try
        {
            if ( data[0] != DBNull.Value )
                entity.Table = new VirtualObjects.Scaffold.VirtualObjectsHelper.Table { Id = (Int32)(Parse(data[0]) ?? default(Int32)) };
        }
        catch ( InvalidCastException )
        {
            //entity.Table = (Table)Convert.ChangeType(new VirtualObjects.Scaffold.VirtualObjectsHelper.Table { Id  = (Int32)(Parse(data[0]) ?? default(Table)(Int32)) }, typeof(Table));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [Table] with [" + data[0] + "] value.", ex);
        }

        try
        {
            if ( data[1] != DBNull.Value )
                entity.Column = new VirtualObjects.Scaffold.VirtualObjectsHelper.Column { Id = (Int32)(Parse(data[1]) ?? default(Int32)) };
        }
        catch ( InvalidCastException )
        {
            //entity.Column = (Column)Convert.ChangeType(new VirtualObjects.Scaffold.VirtualObjectsHelper.Column { Id  = (Int32)(Parse(data[1]) ?? default(Column)(Int32)) }, typeof(Column));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [Column] with [" + data[1] + "] value.", ex);
        }

        try
        {
            if ( data[2] != DBNull.Value )
                entity.ReferencedColumn = new VirtualObjects.Scaffold.VirtualObjectsHelper.Column { Id = (Int32)(Parse(data[2]) ?? default(Int32)) };
        }
        catch ( InvalidCastException )
        {
            //entity.ReferencedColumn = (Column)Convert.ChangeType(new VirtualObjects.Scaffold.VirtualObjectsHelper.Column { Id  = (Int32)(Parse(data[2]) ?? default(Column)(Int32)) }, typeof(Column));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [ReferencedColumn] with [" + data[2] + "] value.", ex);
        }

        try
        {
            if ( data[3] != DBNull.Value )
                entity.ReferencedTable = new VirtualObjects.Scaffold.VirtualObjectsHelper.Table { Id = (Int32)(Parse(data[3]) ?? default(Int32)) };
        }
        catch ( InvalidCastException )
        {
            //entity.ReferencedTable = (Table)Convert.ChangeType(new VirtualObjects.Scaffold.VirtualObjectsHelper.Table { Id  = (Int32)(Parse(data[3]) ?? default(Table)(Int32)) }, typeof(Table));
        }
        catch ( Exception ex )
        {
            throw new Exception("Error setting value to [ReferencedTable] with [" + data[3] + "] value.", ex);
        }


        return new MapResult
        {
            Entity = entity
        };
    }

    public static Object EntityCast(Object source)
    {
        return source;
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