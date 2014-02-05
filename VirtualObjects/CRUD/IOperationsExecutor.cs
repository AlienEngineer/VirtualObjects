using System;

namespace VirtualObjects.CRUD
{
    public interface IOperationsExecutor
    {
        Object Insert(Object entityModel, SessionContext sessionContext);
        Object Update(Object entityModel, SessionContext sessionContext);
        Object Delete(Object entityModel, SessionContext sessionContext);
        Object Get(Object entityModel, SessionContext sessionContext);
    }

    public interface IOperationsExecutor<T>
    {
        T Insert(T entityModel, SessionContext sessionContext);
        T Update(T entityModel, SessionContext sessionContext);
        T Delete(T entityModel, SessionContext sessionContext);
        T Get(T entityModel, SessionContext sessionContext);
    }
}
