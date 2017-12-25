using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Framework.FrameDependency
{
    internal interface IFrameDependencyManager<DependencyType, DescriptorType>
		where DependencyType : IDependency<DescriptorType>
		where DescriptorType : IDependencyDescriptor
	{

		/// <summary>
		/// Dependencies.
		/// </summary>
		IEnumerable<DependencyType> Dependencies { get; }


		/// <summary>
		/// Add a dependency to the manager.
		/// </summary>
		/// <param name="dependency">Dependency to add.</param>
		void AddDependency(DependencyType dependency);

		/// <summary>
		/// Remove a dependency to the manager.
		/// </summary>
		/// <param name="dependency">Dependency to remove.</param>
		void RemoveDependency(DependencyType dependency);

		/// <summary>
		/// Set a dependency as loaded from the loader thread.
		/// </summary>
		/// <param name="frame">Frame when is loaded.</param>
		void SetDependencyLoaded(long descriptorId, long frame);

		/// <summary>
		/// Gets all pending dependencies.
		/// </summary>
		/// <returns></returns>
		IEnumerable<DescriptorType> GetDependencyDescriptors();
	}
}
