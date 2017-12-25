using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Framework.Binding
{
    /// <summary>
    /// Makes this property bindable.
    /// </summary>
    /// <remarks>
    /// This works only inside <see cref="pObject"/> and for properties.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class Bindable : Attribute
    {
        /// <summary>
        /// Binding direction.
        /// </summary>
        public BindingMode Direction = BindingMode.TwoWay;

        /// <summary>
        /// Makes this property bindable.
        /// </summary>
        public Bindable()
        {
        }

    }
}
