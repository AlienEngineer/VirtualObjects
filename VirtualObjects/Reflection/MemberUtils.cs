using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VirtualObjects.Reflection
{
    internal static class MemberUtils
    {
        /// <summary>
        /// Gets all members of the given <paramref name="memberTypes"/> on the given <paramref name="type"/> that 
        /// match the specified <paramref name="bindingFlags"/>, optionally filtered by the supplied <paramref name="names"/>
        /// list (in accordance with the given <paramref name="bindingFlags"/>).
        /// </summary>
        /// <param name="type">The type to reflect on.</param>
        /// <param name="memberTypes">The <see href="MemberTypes"/> to include in the result.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
        /// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
        /// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
        /// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
        /// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching members on the type. This value will never be null.</returns>
        private static IList<MemberInfo> Members(this Type type, MemberTypes memberTypes, Flags bindingFlags,
                                                 params string[] names)
        {
            if (type == null || type == typeof(object))
            {
                return new MemberInfo[0];
            }

            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);
            bool hasNames = names != null && names.Length > 0;
            bool hasSpecialFlags = bindingFlags.IsAnySet(Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented | Flags.ExcludeHiddenMembers);

            if (!recurse && !hasNames && !hasSpecialFlags)
            {
                return type.FindMembers(memberTypes, bindingFlags, null, null);
            }

            var members = GetMembers(type, memberTypes, bindingFlags);
            members = hasSpecialFlags ? members.Filter(bindingFlags) : members;
            members = hasNames ? members.Filter(bindingFlags, names) : members;
            return members;
        }

        private static IList<MemberInfo> GetMembers(Type type, MemberTypes memberTypes, Flags bindingFlags)
        {
            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);

            if (!recurse)
            {
                return type.FindMembers(memberTypes, bindingFlags, null, null);
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var members = new List<MemberInfo>();
            members.AddRange(type.FindMembers(memberTypes, bindingFlags, null, null));
            Type baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                members.AddRange(baseType.FindMembers(memberTypes, bindingFlags, null, null));
                baseType = baseType.BaseType;
            }
            return members;
        }

        /// <summary>
        /// Gets the member identified by <paramref name="name"/> on the given <paramref name="type"/>. Use 
        /// the <paramref name="bindingFlags"/> parameter to define the scope of the search.
        /// </summary>
        /// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
        private static MemberInfo Member(this Type type, string name, Flags bindingFlags)
        {
            // we need to check all members to do partial name matches
            if (bindingFlags.IsAnySet(Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented))
            {
                return type.Members(MemberTypes.All, bindingFlags, name).FirstOrDefault();
            }

            IList<MemberInfo> result = type.GetMember(name, bindingFlags);
            bool hasSpecialFlags = bindingFlags.IsAnySet(Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented | Flags.ExcludeHiddenMembers);
            result = hasSpecialFlags && result.Count > 0 ? result.Filter(bindingFlags) : result;
            bool found = result.Count > 0;

            if (!found && bindingFlags.IsNotSet(Flags.DeclaredOnly))
            {
                if (type.BaseType != typeof(object) && type.BaseType != null)
                {
                    return type.BaseType.Member(name, bindingFlags);
                }
            }
            return found ? result[0] : null;
        }
        /// <summary>
        /// Determines whether the given <paramref name="member"/> is a static member.
        /// </summary>
        /// <returns>True for static fields, properties and methods and false for instance fields,
        /// properties and methods. Throws an exception for all other <see href="MemberTypes" />.</returns>
        public static bool IsStatic(this MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.IsStatic;
            var property = member as PropertyInfo;
            if (property != null)
                return property.CanRead ? property.GetGetMethod(true).IsStatic : property.GetSetMethod(true).IsStatic;
            var method = member as MethodInfo;
            if (method != null)
                return method.IsStatic;
            string message = string.Format("Unable to determine IsStatic for member {0}.{1}" +
                "MemberType was {2} but only fields, properties and methods are supported.",
                member.Name, member.MemberType, Environment.NewLine);
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// Compares the signature of the method with the given parameter types and returns true if
        /// all method parameters have the same order and type. Parameter names are not considered.
        /// </summary>
        /// <returns>True if the supplied parameter type array matches the method parameters array, false otherwise.</returns>
        public static bool HasParameterSignature(this MethodBase method, ParameterInfo[] parameters)
        {
            return method.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameters.Select(p => p.ParameterType));
        }

        /// <summary>
        /// Gets the system type of the field or property identified by the <paramref name="member"/>.
        /// </summary>
        /// <returns>The system type of the member.</returns>
        public static Type Type(this MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
            {
                return field.FieldType;
            }
            var property = member as PropertyInfo;
            if (property != null)
            {
                return property.PropertyType;
            }
            throw new NotSupportedException("Can only determine the type for fields and properties.");
        }

    }
}
