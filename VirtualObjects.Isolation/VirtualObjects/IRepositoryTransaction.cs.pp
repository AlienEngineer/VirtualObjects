namespace $rootnamespace$
{
    public interface IRepositoryTransaction
    {
        /// <summary>
        /// Rollback any changes made during this transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Commit any changes made during this transaction.
        /// </summary>
        void Commit();
    }
}