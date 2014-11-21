using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VirtualObjects.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="this">The this.</param>
    /// <param name="value">The value.</param>
    public delegate void MemberSetter(object @this, object value);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="this">The this.</param>
    /// <returns></returns>
    public delegate object MemberGetter(object @this);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
    public delegate object ConstructorInvoker(params object[] parameters);

    internal static class TypeUtils
    {
        public static Type[] ToTypeArray(this ParameterInfo[] parameters)
        {
            if (parameters.Length == 0)
                return Type.EmptyTypes;
            var types = new Type[parameters.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }
            return types;
        }

        public static Type[] ToTypeArray(this object[] objects)
        {
            if (objects.Length == 0)
                return Type.EmptyTypes;
            var types = new Type[objects.Length];
            for (int i = 0; i < types.Length; i++)
            {
                var obj = objects[i];
                types[i] = obj != null ? obj.GetType() : null;
            }
            return types;
        }

        private static readonly List<byte[]> tokens = new List<byte[]>
		                                              {
		                                              	new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 },
		                                              	new byte[] { 0x31, 0xbf, 0x38, 0x56, 0xad, 0x36, 0x4e, 0x35 },
		                                              	new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
		                                              };

        internal class ByteArrayEqualityComparer : EqualityComparer<byte[]>
        {
            public override bool Equals(byte[] x, byte[] y)
            {
                return x != null && y != null && x.SequenceEqual(y);
            }

            public override int GetHashCode(byte[] obj)
            {
                return obj.GetHashCode();
            }
        }

        public static bool IsFrameworkType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            byte[] publicKeyToken = type.Assembly.GetName().GetPublicKeyToken();
            return publicKeyToken != null && tokens.Contains(publicKeyToken, new ByteArrayEqualityComparer());
        }

        /// <summary>
        /// Returns true of the supplied <paramref name="type" /> inherits from or implements the type <typeparam name="T"></typeparam>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// True if the given type inherits from or implements the specified base type.
        /// </returns>
        public static bool InheritsOrImplements<T>(this Type type)
        {
            return type.InheritsOrImplements(typeof(T));
        }

        /// <summary>
        /// Returns true of the supplied <paramref name="type"/> inherits from or implements the type <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The base type to check for.</param>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the given type inherits from or implements the specified base type.</returns>
        public static bool InheritsOrImplements(this Type type, Type baseType)
        {
            if (type == null || baseType == null)
                return false;
            return baseType.IsInterface ? type.Implements(baseType) : type.Inherits(baseType);
        }

        /// <summary>
        /// Returns true of the supplied <paramref name="type"/> implements the given interface <paramref name="interfaceType"/>. If the given
        /// interface type is a generic type definition this method will use the generic type definition of any implemented interfaces
        /// to determine the result.
        /// </summary>
        /// <param name="interfaceType">The interface type to check for.</param>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the given type implements the specified interface.</returns>
        /// <remarks>This method is for interfaces only. Use <seealso cref="Inherits"/> for classes and <seealso cref="InheritsOrImplements"/> 
        /// to check both interfaces and classes.</remarks>
        public static bool Implements(this Type type, Type interfaceType)
        {
            if (type == null || interfaceType == null || type == interfaceType)
                return false;
            if (interfaceType.IsGenericTypeDefinition && type.GetInterfaces().Where(t => t.IsGenericType).Select(t => t.GetGenericTypeDefinition()).Any(gt => gt == interfaceType))
            {
                return true;
            }
            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Returns true if the supplied <paramref name="type"/> inherits from the given class <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The type (class) to check for.</param>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the given type inherits from the specified class.</returns>
        /// <remarks>This method is for classes only. Use <seealso cref="Implements"/> for interface types and <seealso cref="InheritsOrImplements"/> 
        /// to check both interfaces and classes.</remarks>
        public static bool Inherits(this Type type, Type baseType)
        {
            if (baseType == null || type == null || type == baseType)
                return false;
            var rootType = typeof(object);
            if (baseType == rootType)
                return true;
            while (type != null && type != rootType)
            {
                var current = type.IsGenericType && baseType.IsGenericTypeDefinition ? type.GetGenericTypeDefinition() : type;
                if (baseType == current)
                    return true;
                type = type.BaseType;
            }
            return false;
        }
    }
}
