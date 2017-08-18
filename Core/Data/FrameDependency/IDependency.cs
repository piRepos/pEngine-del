using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Data.FrameDependency
{
    public interface IDependency<DescriptorType> : IDisposable, IDependencyReference where DescriptorType : IDependencyDescriptor
	{
		/// <summary>
		/// Sets the dependency modified.
		/// </summary>
		void InvalidateDependency();

		/// <summary>
		/// Generate a descriptor for this dependency.
		/// This descriptor will be used as frame resource.
		/// </summary>
		/// <returns>The dependency descriptor.</returns>
		DescriptorType GetDescriptor();

		/// <summary>
		/// Actual dependency load state.
		/// </summary>
		DependencyState State { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
        new long DependencyID { get; set; }
	}

	/// <summary>
	/// Possibile dependency states.
	/// </summary>
	public enum DependencyState
	{
		/// <summary>
		/// The dependency is not loaded.
		/// </summary>
		NotLoaded = 0,

		/// <summary>
		/// The dependency is loaded from the worker thread.
		/// </summary>
		Loaded = 1,

		/// <summary>
		/// This dependency has some proprty modified, needs reload.
		/// </summary>
		Modified = 2,

		/// <summary>
		/// This dependency is disposed, before release it we must delete
		/// all references on the worker thread.
		/// </summary>
		Disposed = 3
	}
}
