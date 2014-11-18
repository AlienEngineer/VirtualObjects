using System;
using System.Linq;
using System.Reflection;

namespace VirtualObjects.Reflection
{
    internal class LookupUtils
    {
        public static ConstructorInfo GetConstructor(CallInfo callInfo)
        {
            var constructor = callInfo.MemberInfo as ConstructorInfo;
            if (constructor != null)
                return constructor;

            constructor = callInfo.TargetType.GetConstructor(callInfo.BindingFlags, null, callInfo.ParamTypes, null);
            if (constructor == null)
                throw new MissingMemberException("Constructor does not exist");
            callInfo.MemberInfo = constructor;
            callInfo.MethodParamTypes = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            return constructor;
        }

        public static MemberInfo GetMember(CallInfo callInfo)
        {
            var member = callInfo.MemberInfo;
            if (member != null)
                return member;

            if (callInfo.MemberTypes == MemberTypes.Property)
            {
                member = callInfo.TargetType.Property(callInfo.Name, callInfo.BindingFlags);
                if (member == null)
                {
                    const string fmt = "No match for property with name {0} and flags {1} on type {2}.";
                    throw new MissingMemberException(string.Format(fmt, callInfo.Name, callInfo.BindingFlags, callInfo.TargetType));
                }
                callInfo.MemberInfo = member;
                return member;
            }
            if (callInfo.MemberTypes == MemberTypes.Field)
            {
                member = callInfo.TargetType.Field(callInfo.Name, callInfo.BindingFlags);
                if (member == null)
                {
                    const string fmt = "No match for field with name {0} and flags {1} on type {2}.";
                    throw new MissingFieldException(string.Format(fmt, callInfo.Name, callInfo.BindingFlags, callInfo.TargetType));
                }
                callInfo.MemberInfo = member;
                return member;
            }
            throw new ArgumentException(callInfo.MemberTypes + " is not supported");
        }
        
        public static MethodInfo GetPropertyGetMethod(PropertyInfo propInfo, CallInfo callInfo)
        {
            var method = propInfo.GetGetMethod();
            if (method != null)
                callInfo.MemberInfo = method;
            return method ?? GetPropertyMethod("get_", "getter", callInfo);
        }

        public static MethodInfo GetPropertySetMethod(PropertyInfo propInfo, CallInfo callInfo)
        {
            var method = propInfo.GetSetMethod();
            if (method != null)
                callInfo.MemberInfo = method;
            return method ?? GetPropertyMethod("set_", "setter", callInfo);
        }

        private static MethodInfo GetPropertyMethod(string infoPrefix, string propertyMethod, CallInfo callInfo)
        {
            var method = callInfo.TargetType.Method(null, callInfo.Name, null, callInfo.BindingFlags);
            if (method == null)
            {
                const string fmt = "No {0} for property {1} with flags {2} on type {3}.";
                throw new MissingFieldException(string.Format(fmt, propertyMethod, callInfo.Name, callInfo.BindingFlags, callInfo.TargetType));
            }
            callInfo.MemberInfo = method;
            return method;
        }
    }
}
