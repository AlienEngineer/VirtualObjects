using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtualObjects.Reflection.Emitter;

namespace VirtualObjects.Reflection
{
    internal static class FieldUtils
    {
        /// <summary>
        /// Gets the field identified by <paramref name="name"/> on the given <paramref name="type"/>. 
        /// Use the <paramref name="bindingFlags"/> parameter to define the scope of the search.
        /// </summary>
        /// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
        public static FieldInfo Field(this Type type, string name, Flags bindingFlags)
        {
            // we need to check all fields to do partial name matches
            if (bindingFlags.IsAnySet(Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented))
            {
                return type.Fields(bindingFlags, name).FirstOrDefault();
            }

            var result = type.GetField(name, bindingFlags);
            if (result == null && bindingFlags.IsNotSet(Flags.DeclaredOnly))
            {
                if (type.BaseType != typeof(object) && type.BaseType != null)
                {
                    return type.BaseType.Field(name, bindingFlags);
                }
            }
            bool hasSpecialFlags = bindingFlags.IsAnySet(Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented | Flags.ExcludeHiddenMembers);
            if (hasSpecialFlags)
            {
                IList<FieldInfo> fields = new List<FieldInfo> { result };
                fields = fields.Filter(bindingFlags);
                return fields.Count > 0 ? fields[0] : null;
            }
            return result;
        }

        /// <summary>
        /// Gets all fields on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
        /// </summary>
        /// <param name="type">The type on which to reflect.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="Flags"/> combination used to define
        /// the search behavior and result filtering.</param>
        /// <param name="names">The optional list of names against which to filter the result. If this parameter is
        /// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
        /// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
        /// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
        /// <see href="Flags.IgnoreCase"/> to ignore case.</param>
        /// <returns>A list of all matching fields on the type. This value will never be null.</returns>
        public static IList<FieldInfo> Fields(this Type type, Flags bindingFlags, params string[] names)
        {
            if (type == null || type == typeof(object))
            {
                return new FieldInfo[0];
            }

            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);
            bool hasNames = names != null && names.Length > 0;
            bool hasSpecialFlags = bindingFlags.IsAnySet(Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented | Flags.ExcludeHiddenMembers);

            if (!recurse && !hasNames && !hasSpecialFlags)
            {
                return type.GetFields(bindingFlags) ?? new FieldInfo[0];
            }

            var fields = GetFields(type, bindingFlags);
            fields = hasSpecialFlags ? fields.Filter(bindingFlags) : fields;
            fields = hasNames ? fields.Filter(bindingFlags, names) : fields;
            return fields;
        }

        private static IList<FieldInfo> GetFields(Type type, Flags bindingFlags)
        {
            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);

            if (!recurse)
            {
                return type.GetFields(bindingFlags) ?? new FieldInfo[0];
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var fields = new List<FieldInfo>();
            fields.AddRange(type.GetFields(bindingFlags));
            Type baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                fields.AddRange(baseType.GetFields(bindingFlags));
                baseType = baseType.BaseType;
            }
            return fields;
        }

        /// <summary>
        /// Gets the value of the instance field identified by <paramref name="fieldInfo"/> on the given <paramref name="obj"/>.
        /// </summary>
        public static object Get(this FieldInfo fieldInfo, object obj)
        {
            return fieldInfo.DelegateForGetFieldValue()(obj);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the field identified by <paramref name="fieldInfo"/>.
        /// </summary>
        private static MemberGetter DelegateForGetFieldValue(this FieldInfo fieldInfo)
        {
            var flags = fieldInfo.IsStatic ? Flags.StaticAnyVisibility : Flags.InstanceAnyVisibility;
            return (MemberGetter)new MemberGetEmitter(fieldInfo, flags).GetDelegate();
        }
    }
}
