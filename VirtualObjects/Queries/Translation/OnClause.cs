using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Fasterflect;
using VirtualObjects.Config;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.CodeGenerators;
using System.Data;

namespace VirtualObjects.Queries.Translation
{
    /// <summary>
    /// Represents an Join OnClause
    /// </summary>
    public class OnClause
    {
        /// <summary>
        /// Gets or sets the column1.
        /// </summary>
        /// <value>
        /// The column1.
        /// </value>
        public IEntityColumnInfo Column1 { get; set; }
        /// <summary>
        /// Gets or sets the column2.
        /// </summary>
        /// <value>
        /// The column2.
        /// </value>
        public IEntityColumnInfo Column2 { get; set; }
    }
}
