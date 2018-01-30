using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PropertyChanged;

#pragma warning disable CS0067

namespace pEngine.Framework
{
    /// <summary>
    /// pEngine framework base object.
    /// </summary>
    public partial class pObject : INotifyPropertyChanged, IDisposable
    {
		/// <summary>
		/// True if this object is disposed.
		/// </summary>
		public bool Disposed { get; private set; }

        /// <summary>
        /// Makes a new instance of <see cref="pObject"/> class.
        /// </summary>
        public pObject()
        {
            initializeBinding();
			initializeValidations();
			initializeCacheModule();
		}

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		~pObject()
		{
			Dispose(false);
		}

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		public void Dispose()
        {
			Dispose(true);

			GC.SuppressFinalize(this);
        }

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		/// <param name="disposing">Dispose managed resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (Disposed)
				return;

			if (disposing)
			{
				disposeBinding();
				disposeCacheModule();
			}

			// Free any unmanaged objects here.
			//
			Disposed = true;
		}

		/// <summary>
		/// Triggered on a property change.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
    }
}
