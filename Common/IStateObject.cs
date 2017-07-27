using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine
{
    public interface IStateObject
    {
		/// <summary>
		/// Object load state.
		/// </summary>
		LoadState State { get; }

		/// <summary>
		/// True if this object is loaded.
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Triggered on object load.
		/// </summary>
		event EventHandler OnLoad;
    }

	public enum LoadState
	{
		/// <summary>
		/// Object not loaded.
		/// </summary>
		NotLoaded,

		/// <summary>
		/// Object is now loading.
		/// </summary>
		Loading,

		/// <summary>
		/// Object is ready.
		/// </summary>
		Loaded,

		/// <summary>
		/// Object is ready.
		/// </summary>
		Alive,

		/// <summary>
		/// Object is in dispose state.
		/// </summary>
		Disposing
	}
}
