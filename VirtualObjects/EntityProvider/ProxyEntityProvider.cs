using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using Fasterflect;
using VirtualObjects.Config;
using VirtualObjects.Exceptions;

namespace VirtualObjects.EntityProvider
{
    class ProxyEntityProvider : EntityModelProvider
    {
        private readonly ProxyGenerator _proxyGenerator;
        private ProxyGenerationOptions _proxyGenerationOptions;

        public ProxyEntityProvider(ProxyGenerator proxyGenerator)
        {
            _proxyGenerator = proxyGenerator;
        }

        internal ProxyEntityProvider() : this(new ProxyGenerator())
        {
            
        }

        public override object CreateEntity(Type type)
        {
            Debug.Assert(type != null, "type != null");

// ReSharper disable once PossibleNullReferenceException
            while ( type.GetInterfaces().Contains(typeof(IProxyTargetAccessor)) )
            {
                type = type.BaseType;
            }

            return _proxyGenerator.CreateClassProxy(type, _proxyGenerationOptions);
        }


        public override bool CanCreate(Type type)
        {
            if (type.IsDynamic()) return false;
            if (type.IsFrameworkType()) return false;
            if (type.InheritsOrImplements<IEnumerable>()) return false;
            
            return (type.Properties().Any(e => e.GetGetMethod().IsVirtual));
        }

        public override void PrepareProvider(Type outputType, SessionContext sessionContext)
        {
            _proxyGenerationOptions = new ProxyGenerationOptions
            {
                Selector = new InterceptorSelector(
                    new NonCollectionInterceptor(sessionContext.Session),
                    new CollectionPropertyInterceptor(sessionContext.Session, sessionContext.Mapper)
                )
            };
        }
    }

    interface IFieldInterceptor : IInterceptor
    {
        Boolean InterceptCollections { get; }
    }

    class InterceptorSelector : IInterceptorSelector
    {
        private readonly IFieldInterceptor[] _interceptors;

        public InterceptorSelector(params IFieldInterceptor[] interceptors)
        {
            _interceptors = interceptors;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var isCollection =
                IsCollectionReturnType(method) ||
                IsCollectionParameterType(method);

            return _interceptors
                .Where(e => e.InterceptCollections == isCollection)
                .Cast<IInterceptor>()
                .ToArray();
        }

        private static bool IsCollectionParameterType(MethodInfo method)
        {
            return method.Name.StartsWith("set_") &&
                   method.Parameters().First().ParameterType.GetGenericArguments().Any();
        }

        private static bool IsCollectionReturnType(MethodInfo method)
        {
            return method.Name.StartsWith("get_") && method.ReturnType.IsGenericType;
        }
    }

    abstract class FieldInterceptorBase : IFieldInterceptor
    {

        internal class PropertyValue
        {
            public object Value { get; set; }

            public int SettedCount { get; set; }

            public Boolean IsLoaded { get; set; }
        }

        public static int Typecount = 0;
        private readonly IDictionary<MethodInfo, PropertyValue> _properties = new Dictionary<MethodInfo, PropertyValue>();

        protected FieldInterceptorBase(Boolean interceptCollections)
        {
            InterceptCollections = interceptCollections;
            ++Typecount;
        }

        public Boolean InterceptCollections { get; private set; }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var method = invocation.Method;

            if ( method.Name.StartsWith("set_") )
            {
                HandleSetter(method, invocation);
            }
            else
            {
                if ( method.Name.StartsWith("get_") )
                {
                    HandleGetter(method, invocation);
                }
            }
        }

        private void HandleSetter(MethodInfo method, IInvocation invocation)
        {
            var getter = invocation.TargetType.GetMethod("g" + method.Name.Remove(0, 1));

            PropertyValue propValue;
            if ( _properties.TryGetValue(getter, out propValue) )
            {
                propValue.Value = invocation.GetArgumentValue(0);
                return;
            }

            _properties[getter] = new PropertyValue
            {
                Value = invocation.GetArgumentValue(0),

                IsLoaded = false
            };
        }

        private void HandleGetter(MethodInfo method, IInvocation invocation)
        {
            PropertyValue propValue;
            if ( _properties.TryGetValue(method, out propValue) && propValue.IsLoaded )
            {
                invocation.ReturnValue = propValue.Value;
                return;
            }

            var value = GetFieldValue(method, invocation);

            if ( propValue == null )
            {
                _properties[method] = new PropertyValue
                {
                    IsLoaded = true,
                    Value = value
                };
            }

            invocation.ReturnValue = value;
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        protected abstract object GetFieldValue(MethodInfo method, IInvocation invocation);
    }

    class NonCollectionInterceptor : FieldInterceptorBase
    {
        private readonly ISession _session;
        private readonly MethodInfo _methodInfo;

        public NonCollectionInterceptor(ISession session)
            : base(false)
        {
            _methodInfo = typeof(ISession).Method("GetById");
            _session = session;
        }

        protected override object GetFieldValue(MethodInfo method, IInvocation invocation)
        {
            var genericMethod = _methodInfo
                .MakeGenericMethod(method.ReturnType);

            return genericMethod.Invoke(_session, new[] { invocation.ReturnValue });
        }
    }

    class CollectionPropertyInterceptor : FieldInterceptorBase
    {
        private static readonly MethodInfo ProxyGenericIteratorMethod =
            typeof(CollectionPropertyInterceptor).GetMethod("ProxyGenericIterator", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo SessionGenericGetAllMethod =
            typeof(ISession).GetMethod("GetAll", BindingFlags.Public | BindingFlags.Instance);

        private static readonly MethodInfo WhereMethod = 
            typeof(CollectionPropertyInterceptor).GetMethod("Where");

        private readonly ISession _session;
        private readonly IMapper _mapper;

        public CollectionPropertyInterceptor(ISession session, IMapper mapper)
            : base(true)
        {
            _mapper = mapper;
            _session = session;
        }


// ReSharper disable once UnusedMember.Local
        private static IEnumerable<T> ProxyGenericIterator<T>(object target, IEnumerable enumerable)
        {
            return ProxyNonGenericIterator(target, enumerable).Cast<T>();
        }

        private static IEnumerable ProxyNonGenericIterator(object target, IEnumerable enumerable)
        {
            return enumerable.Cast<object>();
        }

        private object BuildWhereClause(IInvocation invocation, Type entityType, IQueryable result, IEntityInfo foreignTable, IEntityInfo callerTable)
        {
            var parameter = Expression.Parameter(entityType, "e");
            BinaryExpression last = null;

            foreach ( var key in callerTable.KeyColumns )
            {
                var foreignField = foreignTable.ForeignKeys
                    .FirstOrDefault(f => f.BindOrName.Equals(key.ColumnName, StringComparison.InvariantCultureIgnoreCase));

                if ( foreignField != null )
                {
                    var value = Expression.Constant(
                        key.GetFieldFinalValue(invocation.InvocationTarget), key.Property.PropertyType);

                    var member = Expression.Property(parameter, foreignField.Property);

                    member = CreateFullMemberAccess(member, foreignField);

                    if ( last == null )
                    {
                        last = Expression.Equal(member, value);
                    }
                    else
                    {
                        var equal = Expression.Equal(member, value);
                        last = Expression.AndAlso(last, equal);
                    }
                }
            }

            if ( last == null )
            {
                throw new VirtualObjectsException(Errors.Configuration_UnableToBindCollection, new { callerTable.EntityName});
            }

            
            var where = Expression.Call(null,
            WhereMethod.MakeGenericMethod(entityType),
                Expression.Constant(result),
                Expression.Lambda(last, parameter)
            );

            result = result.Provider.CreateQuery(where);

            return result;
        }

        protected override object GetFieldValue(MethodInfo method, IInvocation invocation)
        {
            Debug.Assert(method.ReturnParameter != null, "method.ReturnParameter != null");

            var entityType = method.ReturnParameter.ParameterType.GetGenericArguments().First();

            var methodIterator = ProxyGenericIteratorMethod.MakeGenericMethod(entityType);

            var me = SessionGenericGetAllMethod.MakeGenericMethod(entityType);

            var baseQuery = (IQueryable)methodIterator.Invoke(null, new[]
            {
                invocation.InvocationTarget,
                me.Invoke(_session, new Object[] { })
            });

            // The generic type collection table representation.
            var foreignTable = _mapper.Map(entityType);

            // The table representation of the caller.
            var callerTable = _mapper.Map(invocation.Method.ReflectedType);
            
            return BuildWhereClause(invocation, entityType, baseQuery, foreignTable, callerTable);
        }

        public static IQueryable<T> Where<T>(object query, Object lambda)
        {
            return null;
        }

        private MemberExpression CreateFullMemberAccess(MemberExpression member, IEntityColumnInfo foreignField)
        {
            if ( foreignField == null || foreignField.ForeignKey == null )
            {
                return member;
            }

            return CreateFullMemberAccess(Expression.Property(member, foreignField.ForeignKey.Property), foreignField.ForeignKey);
        }
    }
}
