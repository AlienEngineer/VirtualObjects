using System.Data;

namespace VirtualObjects.Core.Connection
{
    class InnerTransaction : ITranslation
    {
        private readonly ITranslation _translation;

        public InnerTransaction(ITranslation translation)
        {
            _translation = translation;
        }

        public IDbConnection DbConnection
        {
            get { return _translation.DbConnection; }
        }

        public void Rollback()
        {
            _translation.Rollback();
        }

        public void Commit()
        {

        }

    }
}