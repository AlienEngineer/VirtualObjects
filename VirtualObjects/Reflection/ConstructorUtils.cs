using System;
using VirtualObjects.Reflection.Emitter;

namespace VirtualObjects.Reflection
{
    internal static class ConstructorUtils
    {

        /// <summary>
        /// Invokes a constructor whose parameter types are inferred from <paramref name="parameters" /> 
        /// on the given <paramref name="type"/> with <paramref name="parameters" /> being the arguments.
        /// Leave <paramref name="parameters"/> empty if the constructor has no argument.
        /// </summary>
        /// <remarks>
        /// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
        /// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
        /// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
        /// </remarks>
        /// <seealso cref="CreateInstance(Type, Type[], object[])"/>
        public static object CreateInstance(this Type type, params object[] parameters)
        {
            return DelegateForCreateInstance(type, parameters.ToTypeArray())(parameters);
        }

        /// <summary>
        /// Invokes a constructor having parameter types specified by <paramref name="parameterTypes" /> 
        /// on the the given <paramref name="type"/> with <paramref name="parameters" /> being the arguments.
        /// </summary>
        public static object CreateInstance(this Type type, Type[] parameterTypes, params object[] parameters)
        {
            return DelegateForCreateInstance(type, parameterTypes)(parameters);
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
        /// on the given <paramref name="type"/>.  Leave <paramref name="parameterTypes"/> empty if the constructor
        /// has no argument.
        /// </summary>
        private static ConstructorInvoker DelegateForCreateInstance(this Type type, params Type[] parameterTypes)
        {
            return DelegateForCreateInstance(type, Flags.InstanceAnyVisibility, parameterTypes);
        }

        /// <summary>
        /// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
        /// and matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.  
        /// Leave <paramref name="parameterTypes"/> empty if the constructor has no argument. 
        /// </summary>
        private static ConstructorInvoker DelegateForCreateInstance(this Type type, Flags bindingFlags,  params Type[] parameterTypes)
        {
            return (ConstructorInvoker)new CtorInvocationEmitter(type, bindingFlags, parameterTypes).GetDelegate();
        }
    }
}
