using System;

namespace VirtualObjects.Core.CRUD
{
    public interface IOperationsExecutor
    {
        Object Insert(Object entityModel, IConnection connection);
        Object Update(Object entityModel, IConnection connection);
        Object Delete(Object entityModel, IConnection connection);
        Object Get(Object entityModel, IConnection connection);
    }

    public interface IOperationsExecutor<T>
    {
        T Insert(T entityModel, IConnection connection);
        T Update(T entityModel, IConnection connection);
        T Delete(T entityModel, IConnection connection);
        T Get(T entityModel, IConnection connection);
    }
}
