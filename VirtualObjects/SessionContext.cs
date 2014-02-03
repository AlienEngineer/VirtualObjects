using System.Linq;
using VirtualObjects.Config;

namespace VirtualObjects
{
    public class SessionContext
    {
        public IQueryProvider QueryProvider { get; set; }
        public IMapper Mapper { get; set; }
        public IConnection Connection { get; set; }

        public IEntityInfo Map<TEntity>()
        {
            return Mapper.Map(typeof (TEntity));
        }
    }
}
