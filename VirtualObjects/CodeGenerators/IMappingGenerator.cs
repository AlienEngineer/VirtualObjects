using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    /// <summary>
    /// Creates a method to map an entity.
    /// </summary>
    public interface IMappingGenerator
    {

        /// <summary>
        /// Generates the mapper.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns></returns>
        Action<Object, Object[]> GenerateMapper(IEntityInfo entityInfo);

    }
}
