﻿using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Exceptions;

namespace VirtualObjects.CRUD.Operations
{
    abstract class Operation : IOperation
    {
        private readonly IEntityInfo _entityInfo;
        private IDictionary<String, IOperationParameter> _parameters;
        private object _entityModel;

        protected Operation(String commandText, IEntityInfo entityInfo)
        {
            CommandText = commandText;
            _entityInfo = entityInfo;
        }

        public string CommandText { get; private set; }
        
        public object Execute(SessionContext sessionContext)
        {
            return Execute(
                sessionContext.Connection, 
                _entityModel, 
                _entityInfo, 
                CommandText, 
                _parameters, 
                sessionContext
            );
        }

        public IOperation PrepareOperation(object entityModel)
        {
            _entityModel = entityModel;
            try
            {
                _parameters = GetParameters(_entityInfo)
                .Select(e => new
                {
                    Key = e.ColumnName,
                    Value = e.GetFieldFinalValue(entityModel),
                    e.Property,
                    Column = e
                })
                .ToDictionary(
                    e => e.Key,
                    e => (IOperationParameter)new OperationParameter
                    {
                        Type = e.Property.PropertyType,
                        Value = e.Value,
                        Name = e.Key,
                        Column = e.Column
                    });
            }
            catch (Exception ex)
            {
                throw new VirtualObjectsException("Unable to create parameters for {EntityName}.", _entityInfo, ex);
            }
            

            return this;
        }

        protected abstract object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext);
        protected abstract IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo);
    }
}