using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Framework.Binding
{
    /// <summary>
    /// Specifies the binding direction.
    /// </summary>
    [Flags]
    public enum BindingMode
    {
        /// <summary>
        /// The binding will update from the destination to the source.
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// The binding will update from the source to the destination.
        /// </summary>
        WriteOnly = 2,

        /// <summary>
        /// The binding will update in both directions.
        /// </summary>
        TwoWay = ReadOnly | WriteOnly
    }
}
