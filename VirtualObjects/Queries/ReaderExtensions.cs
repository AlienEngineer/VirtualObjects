using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VirtualObjects.Queries.ConcurrentReader;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// 
    /// </summary>
    static class ReaderExtensions
    {
        /// <summary>
        /// Gets the column names.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static String[] GetColumnNames(this IDataReader reader)
        {
            var result = new String[reader.FieldCount];

            for ( var i = 0; i < result.Length; i++ )
            {
                result[i] = reader.GetName(i).ToLower();
            }

            return result;
        }

        

        /// <summary>
        /// Makes this reader into a Thread Safe reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="readWhile">The read while.</param>
        /// <returns></returns>
        public static IConcurrentDataReader AsParallel(this IDataReader reader, Predicate<IDataReader> readWhile = null)
        {
            return new BlockingDataReader(reader, readWhile);
        }

        /// <summary>
        /// Parallels for each.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        /// <param name="maxThreads">The maximum threads.</param>
        /// <returns></returns>
        public static IEnumerable<ITuple> ParallelForEach(this IDataReader reader, Action<IConcurrentDataReader> action,
                                                          int maxThreads)
        {
            return reader.AsParallel().ForEach(action, maxThreads);
        }

        /// <summary>
        /// Parallels for each.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<ITuple> ParallelForEach(this IDataReader reader, Action<IConcurrentDataReader> action)
        {
            return reader.AsParallel().ForEach(action);
        }

        /// <summary>
        /// Parallels the transform.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="maxThreads">The maximum threads.</param>
        /// <returns></returns>
        public static IEnumerable<TModel> ParallelTransform<TModel>(this IConcurrentDataReader reader,
                                                                    Func<ITuple, TModel> transform, int maxThreads)
            where TModel: class,
            new()
        {
            return reader.AsParallel().Transform(transform, maxThreads);
        }

        /// <summary>
        /// Parallels the transform.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="transform">The transform.</param>
        /// <returns></returns>
        public static IEnumerable<TModel> ParallelTransform<TModel>(this IConcurrentDataReader reader,
                                                                    Func<ITuple, TModel> transform)
            where TModel: class,
            new()
        {
            return reader.AsParallel().Transform(transform);
        }

        /// <summary>
        /// Iterates the reader and calls the action for every record.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        /// <param name="maxThreads">The max threads.</param>
        /// <returns></returns>
        public static IEnumerable<ITuple> ForEach(this IConcurrentDataReader reader,
                                                  Action<IConcurrentDataReader> action, int maxThreads)
        {
            var ts = new HashSet<Task>();

            for ( var i = 0; i < maxThreads; i++ )
            {
                ts.Add(Task.Factory.StartNew(() =>
                {
                    while ( reader.Read() )
                    {
                        action(reader);
                    }
                }));
            }

            reader.Close();

            Task.WaitAll(ts.ToArray());

            return reader.GetTuples();
        }

        /// <summary>
        /// Iterates the reader and calls the action for every record. 
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        public static IEnumerable<ITuple> ForEach(this IConcurrentDataReader reader,
                                                  Action<IConcurrentDataReader> action)
        {
            return reader.ForEach(action, Environment.ProcessorCount);
        }

        /// <summary>
        /// Iterates the reader and transforms the ITuple instance into TModel type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="maxThreads">The max threads.</param>
        /// <returns><code>IEnumerable<TModel/></code> With the same order as the records were read.</returns>
        public static IEnumerable<TModel> Transform<TModel>(this IConcurrentDataReader reader,
                                                            Func<ITuple, TModel> transform, int maxThreads)
        {
            var models = new ConcurrentDictionary<ITuple, TModel>();
            reader.ForEach(r =>
            {
                var data = r.GetData();
                models[data] = transform(data);
            }, maxThreads);

            return reader.GetTuples().Select(t => models[t]);
        }

        /// <summary>
        /// Transforms the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="maxThreads">The maximum threads.</param>
        /// <returns></returns>
        public static IEnumerable Transform(this IConcurrentDataReader reader, Type modelType,
                                            Func<ITuple, Object> transform, int maxThreads)
        {
            var models = new ConcurrentDictionary<ITuple, Object>();
            reader.ForEach(r =>
            {
                var data = r.GetData();
                models[data] = Convert.ChangeType(transform(data), modelType);
            }, maxThreads);

            return reader.GetTuples().Select(t => models[t]).ToList();
        }

        /// <summary>
        /// Transforms the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="transform">The transform.</param>
        /// <returns></returns>
        public static IEnumerable Transform(this IConcurrentDataReader reader, Type modelType,
                                            Func<ITuple, Object> transform)
        {
            return reader.Transform(modelType, transform, Environment.ProcessorCount);
        }

        /// <summary>
        /// Iterates the reader and transforms the ITuple instance into TModel type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="transform">The transform.</param>
        /// <returns><code>IEnumerable<TModel/></code> With the same order as the records were read.</returns>
        public static IEnumerable<TModel> Transform<TModel>(this IConcurrentDataReader reader,
                                                            Func<ITuple, TModel> transform)
        {
            return reader.Transform(transform, Environment.ProcessorCount);
        }

        /// <summary>
        /// Cache the readers data.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static IConcurrentDataReader Load(this IConcurrentDataReader reader)
        {
            reader.ForEach(r =>
            {
            });
            return reader;
        }
    }
}
