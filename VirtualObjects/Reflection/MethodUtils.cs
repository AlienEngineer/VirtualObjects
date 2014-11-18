using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VirtualObjects.Reflection
{
    internal static class MethodUtils
    {
        /// <summary>
        /// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
        /// on the given <paramref name="type"/> where the parameter types correspond in order with the
        /// supplied <paramref name="parameterTypes"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="genericTypes">Type parameters if this is a generic method.</param>
        /// <param name="name">The name of the method to search for. This argument must be supplied. The 
        ///   default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
        ///   to include explicitly implemented interface members, <see href="Flags.PartialNameMatch"/> to locate
        ///   by substring, and <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
        ///   will be included in the result. The default behavior is to check only for assignment compatibility,
        ///   but this can be changed to exact matching by passing <see href="Flags.ExactBinding"/>.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        ///   the search behavior and result filtering.</param>
        /// <returns>The specified method or null if no method was found. If there are multiple matches
        /// due to method overloading the first found match will be returned.</returns>
        public static MethodInfo Method(this Type type, Type[] genericTypes, string name, Type[] parameterTypes, Flags bindingFlags)
        {
            bool hasTypes = parameterTypes != null;
            bool hasGenericTypes = genericTypes != null && genericTypes.Length > 0;
            // we need to check all methods to do partial name matches or complex parameter binding
            bool processAll = bindingFlags.IsAnySet(Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented);
            processAll |= hasTypes && bindingFlags.IsSet(Flags.IgnoreParameterModifiers);
            processAll |= hasGenericTypes;
            if (processAll)
            {
                return type.Methods(genericTypes, parameterTypes, bindingFlags, name).FirstOrDefault().MakeGeneric(genericTypes);
            }

            var result = hasTypes
                ? type.GetMethod(name, bindingFlags, null, parameterTypes, null)
                : type.GetMethod(name, bindingFlags);
            if (result == null && bindingFlags.IsNotSet(Flags.DeclaredOnly))
            {
                if (type.BaseType != typeof(object) && type.BaseType != null)
                {
                    return type.BaseType.Method(null, name, parameterTypes, bindingFlags).MakeGeneric(genericTypes);
                }
            }
            bool hasSpecialFlags =
                bindingFlags.IsAnySet(Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented | Flags.ExcludeHiddenMembers);
            if (hasSpecialFlags)
            {
                var methods = new List<MethodInfo> { result }.Filter(bindingFlags);
                return (methods.Count > 0 ? methods[0] : null).MakeGeneric(genericTypes);
            }
            return result.MakeGeneric(genericTypes);
        }

        internal static MethodInfo MakeGeneric(this MethodInfo methodInfo, Type[] genericTypes)
        {
            if (methodInfo == null)
            {
                return null;
            }
            if (genericTypes == null ||
                genericTypes.Length == 0 ||
                genericTypes == Type.EmptyTypes)
            {
                return methodInfo;
            }
            return methodInfo.MakeGenericMethod(genericTypes);
        }

        /// <summary>
        /// Gets all methods on the given <paramref name="type"/> that match the given lookup criteria.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="genericTypes">If this parameter is supplied then only methods with the same generic parameter 
        /// signature will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactBinding"/>.</param>
        /// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
        /// will be included in the result. The default behavior is to check only for assignment compatibility,
        /// but this can be changed to exact matching by passing <see href="Flags.ExactBinding"/>.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
        /// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
        /// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
        /// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
        /// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching methods. This value will never be null.</returns>
        public static IList<MethodInfo> Methods(this Type type, Type[] genericTypes, Type[] parameterTypes, Flags bindingFlags,
            params string[] names)
        {
            if (type == null || type == typeof(object))
            {
                return new MethodInfo[0];
            }
            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);
            bool hasNames = names != null && names.Length > 0;
            bool hasTypes = parameterTypes != null;
            bool hasGenericTypes = genericTypes != null && genericTypes.Length > 0;
            bool hasSpecialFlags =
                bindingFlags.IsAnySet(Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented | Flags.ExcludeHiddenMembers);

            if (!recurse && !hasNames && !hasTypes && !hasSpecialFlags)
            {
                return type.GetMethods(bindingFlags) ?? new MethodInfo[0];
            }

            var methods = GetMethods(type, bindingFlags);
            methods = hasNames ? methods.Filter(bindingFlags, names) : methods;
            methods = hasGenericTypes ? methods.Filter(genericTypes) : methods;
            methods = hasTypes ? methods.Filter(bindingFlags, parameterTypes) : methods;
            methods = hasSpecialFlags ? methods.Filter(bindingFlags) : methods;
            return methods;
        }

        private static IList<MethodInfo> GetMethods(Type type, Flags bindingFlags)
        {
            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);

            if (!recurse)
            {
                return type.GetMethods(bindingFlags) ?? new MethodInfo[0];
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var methods = new List<MethodInfo>();
            methods.AddRange(type.GetMethods(bindingFlags));
            Type baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                methods.AddRange(baseType.GetMethods(bindingFlags));
                baseType = baseType.BaseType;
            }
            return methods;
        }
    }
}
