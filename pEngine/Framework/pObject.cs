using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PropertyChanged;

namespace pEngine.Framework
{
    /// <summary>
    /// pEngine framework base object.
    /// </summary>
    public partial class pObject : INotifyPropertyChanged, IDisposable
    {

        /// <summary>
        /// Makes a new instance of <see cref="pObject"/> class.
        /// </summary>
        public pObject()
        {
            initializeBinding();
        }

        /// <summary>
        /// Dispose all resources used from this class.
        /// </summary>
        public virtual void Dispose()
        {
            disposeBinding();
        }

        /// <summary>
        /// Triggered on a property change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
