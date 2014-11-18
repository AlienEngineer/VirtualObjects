using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtualObjects.Reflection.Emitter;

namespace VirtualObjects.Reflection
{
    internal static class PropertyUtils
    {
        /// <summary>
        /// Creates a delegate which can set the value of the property specified by <param name="name"/>
        /// on the given <param name="type"/>.
        /// </summary>
        private static MemberSetter DelegateForSetPropertyValue(this Type type, string name)
        {
            return DelegateForSetPropertyValue(type, name, Flags.StaticInstanceAnyVisibility);
        }

        /// <summary>
        /// Creates a delegate which can get the value of the property specified by <param name="name"/>
        /// on the given <param name="type"/>.
        /// </summary>
        private static MemberGetter DelegateForGetPropertyValue(this Type type, string name)
        {
            return DelegateForGetPropertyValue(type, name, Flags.StaticInstanceAnyVisibility);
        }

        /// <summary>
        /// Creates a delegate which can set the value of the property specified by <param name="name"/>
        /// matching <param name="bindingFlags"/> on the given <param name="type"/>.
        /// </summary>
        private static MemberSetter DelegateForSetPropertyValue(this Type type, string name, Flags bindingFlags)
        {
            var callInfo = new CallInfo(type, null, bindingFlags, MemberTypes.Property, name, null, null, false);
            return (MemberSetter)new MemberSetEmitter(callInfo).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the property specified by <param name="name"/>
        /// matching <param name="bindingFlags"/> on the given <param name="type"/>.
        /// </summary>
        private static MemberGetter DelegateForGetPropertyValue(this Type type, string name, Flags bindingFlags)
        {
            var callInfo = new CallInfo(type, null, bindingFlags, MemberTypes.Property, name, null, null, true);
            return (MemberGetter)new MemberGetEmitter(callInfo).GetDelegate();
        }


        /// <summary>
        /// Creates a delegate which can set the value of the property <paramref name="propInfo"/>.
        /// </summary>
        public static MemberSetter DelegateForSetPropertyValue(this PropertyInfo propInfo)
        {
            var flags = propInfo.IsStatic() ? Flags.StaticAnyVisibility : Flags.InstanceAnyVisibility;
            return (MemberSetter)new MemberSetEmitter(propInfo, flags).GetDelegate();
        }

        /// <summary>
        /// Creates a delegate which can get the value of the property <param name="propInfo"/>.
        /// </summary>
        public static MemberGetter DelegateForGetPropertyValue(this PropertyInfo propInfo)
        {
            var flags = propInfo.IsStatic() ? Flags.StaticAnyVisibility : Flags.InstanceAnyVisibility;
            return (MemberGetter)new MemberGetEmitter(propInfo, flags).GetDelegate();
        }

        private static IList<PropertyInfo> GetProperties(Type type, Flags bindingFlags)
        {
            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);

            if (!recurse)
            {
                return type.GetProperties(bindingFlags) ?? Constants.EmptyPropertyInfoArray;
            }

            bindingFlags |= Flags.DeclaredOnly;
            bindingFlags &= ~BindingFlags.FlattenHierarchy;

            var properties = new List<PropertyInfo>();
            properties.AddRange(type.GetProperties(bindingFlags));
            Type baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                properties.AddRange(baseType.GetProperties(bindingFlags));
                baseType = baseType.BaseType;
            }
            return properties;
        }
        /// <summary>
        /// Gets all properties on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>,
        /// including properties defined on base types.
        /// </summary>
        /// <returns>A list of all matching properties on the type. This value will never be null.</returns>
        public static IList<PropertyInfo> Properties(this Type type, Flags bindingFlags, params string[] names)
        {
            if (type == null || type == Constants.ObjectType)
            {
                return Constants.EmptyPropertyInfoArray;
            }

            bool recurse = bindingFlags.IsNotSet(Flags.DeclaredOnly);
            bool hasNames = names != null && names.Length > 0;
            bool hasSpecialFlags = bindingFlags.IsAnySet(Flags.ExcludeBackingMembers | Flags.ExcludeExplicitlyImplemented | Flags.ExcludeHiddenMembers);

            if (!recurse && !hasNames && !hasSpecialFlags)
            {
                return type.GetProperties(bindingFlags) ?? Constants.EmptyPropertyInfoArray;
            }

            var properties = GetProperties(type, bindingFlags);
            properties = hasSpecialFlags ? properties.Filter(bindingFlags) : properties;
            properties = hasNames ? properties.Filter(bindingFlags, names) : properties;
            return properties;
        }

        public static PropertyInfo Property(this Type type, string name, Flags bindingFlags)
        {
            // we need to check all properties to do partial name matches
            if (bindingFlags.IsAnySet(Flags.PartialNameMatch | Flags.TrimExplicitlyImplemented))
            {
                return type.Properties(bindingFlags, name).FirstOrDefault();
            }

            var result = type.GetProperty(name, bindingFlags | Flags.DeclaredOnly);
            if (result == null && bindingFlags.IsNotSet(Flags.DeclaredOnly))
            {
                if (type.BaseType != typeof(object) && type.BaseType != null)
                {
                    return type.BaseType.Property(name, bindingFlags);
                }
            }
            bool hasSpecialFlags = bindingFlags.IsSet(Flags.ExcludeExplicitlyImplemented);
            if (hasSpecialFlags)
            {
                IList<PropertyInfo> properties = new List<PropertyInfo> { result };
                properties = properties.Filter(bindingFlags);
                return properties.Count > 0 ? properties[0] : null;
            }
            return result;
        }
    }
}
