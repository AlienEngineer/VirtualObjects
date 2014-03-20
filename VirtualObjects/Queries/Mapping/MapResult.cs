using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Queries.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class MapResult
    {
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public Object Entity { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [has more].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [has more]; otherwise, <c>false</c>.
        /// </value>
        public Boolean HasMore { get; set; }
    }
}
