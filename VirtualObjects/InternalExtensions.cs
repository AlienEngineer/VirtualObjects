using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualObjects.Config;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public static class InternalExtensions
    {

        /// <summary>
        /// Maps the specified mapper.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <returns></returns>
        public static IEntityInfo Map<TEntity>(this IMapper mapper)
        {
            return mapper.Map(typeof(TEntity));
        }

        /// <summary>
        /// Determines whether [has multiple active result sets] [the specified connection].
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public static Boolean HasMultipleActiveResultSets(this IDbConnection connection)
        {
            return connection.ConnectionString
                .Split(';')
                .Reverse() // this usually is set at the end of the connection string.
                .Any(e => 
                    e.StartsWith("MultipleActiveResultSets", StringComparison.InvariantCultureIgnoreCase) && 
                    e.EndsWith("true", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static Object[] GetValues(this IDataReader reader)
        {
            var values = new Object[reader.FieldCount];
            reader.GetValues(values);
            return values;
        }

    }
}
