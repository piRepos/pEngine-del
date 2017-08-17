using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Data.FrameDependency
{
    public abstract class FrameDependencyManager<DependencyType, DescriptorType> : IDisposable
		where DependencyType : IDependency<DescriptorType>
		where DescriptorType : IDependencyDescriptor
    {
		protected pEngine host;

		/// <summary>
		/// Make a new instance of <see cref="FrameDependencyManager{DependencyType, DescriptorType}"/>.
		/// </summary>
		public FrameDependencyManager(pEngine host)
		{
			dependencies = new ConcurrentDictionary<long, DependencyType>();
			validation = new ConcurrentDictionary<long, DescriptorValidation>();

			this.host = host;
		}

		/// <summary>
		/// Releases all resource used by the <see cref="FrameDependencyManager{DependencyType, DescriptorType}"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="FrameDependencyManager{DependencyType, DescriptorType}"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="FrameDependencyManager{DependencyType, DescriptorType}"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="FrameDependencyManager{DependencyType, DescriptorType}"/> so
		/// the garbage collector can reclaim the memory that the <see cref="FrameDependencyManager{DependencyType, DescriptorType}"/> was occupying.</remarks>
		public virtual void Dispose()
		{
		}

		#region Actions

		/// <summary>
		/// Called on new dependency load request.
		/// </summary>
		/// <param name="dependency">Dependency to load.</param>
		abstract protected void OnDependencyLoad(DependencyType dependency);

		/// <summary>
		/// Called on a dependency dispose.
		/// </summary>
		/// <param name="dependency">Dependency to remove.</param>
		abstract protected void OnDependencyDispose(DependencyType dependency);

		/// <summary>
		/// Called on dependency property change.
		/// </summary>
		/// <param name="dependency">Modified dependency.</param>
		abstract protected void OnDependencyChange(DependencyType dependency);

		#endregion

		#region Dependencies

		long key = 0;

		/// <summary>
		/// Internal dependencies.
		/// </summary>
		private ConcurrentDictionary<long, DependencyType> dependencies;

		/// <summary>
		/// Registered dependencies.
		/// </summary>
		public IEnumerable<DependencyType> Dependencies => dependencies.Values;

		/// <summary>
		/// Add a dependency to the manager.
		/// </summary>
		/// <param name="dependency">Dependency to add.</param>
		protected virtual void AddDependency(DependencyType dependency)
		{
			OnDependencyLoad(dependency);

			dependency.DependencyID = key;

			if (dependencies.ContainsKey(dependency.DependencyID))
				return;

			validation[key] = new DescriptorValidation
			{ State = DependencyState.NotLoaded };

			dependencies.TryAdd(key++, dependency);
		}

		#endregion

		#region Descriptors

		/// <summary>
		/// Contains all descriptor status.
		/// </summary>
		private ConcurrentDictionary<long, DescriptorValidation> validation;

		/// <summary>
		/// Set a dependency at the next state from the loader thread.
		/// </summary>
		/// <param name="descriptor">Dependency descriptor.</param>
		public void SetDependency(DescriptorType descriptor)
		{
			if (!validation.ContainsKey(descriptor.DescriptorID))
				throw new InvalidOperationException("This dependency is not avaiable.");

			DependencyState nextState = DependencyState.NotLoaded;
			switch (descriptor.State)
			{
				case DependencyState.NotLoaded:
				case DependencyState.Loaded:
				case DependencyState.Modified:
					nextState = DependencyState.Loaded;
					break;
				case DependencyState.Disposed:
					nextState = DependencyState.Disposed;
					break;
			}

			// - Delete dependency
			if (nextState == DependencyState.NotLoaded)
			{
				DependencyType t;
				dependencies.TryRemove(descriptor.DescriptorID, out t);
				DescriptorValidation d;
				validation.TryRemove(descriptor.DescriptorID, out d);
				return;
			}

			validation[descriptor.DescriptorID] = new DescriptorValidation
			{ State = nextState };
		}

		/// <summary>
		/// Gets all pending dependencies.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<DescriptorType> GetDependencyDescriptors()
		{
			foreach (var dependency in dependencies)
			{
				var va = validation[dependency.Key];

				if (dependency.Value.State == DependencyState.Modified)
					OnDependencyChange(dependency.Value);

				if (dependency.Value.State == DependencyState.Disposed)
					OnDependencyDispose(dependency.Value);

				if (dependency.Value.State != DependencyState.Loaded)
				{
					va.State = dependency.Value.State;
					validation[dependency.Key] = va;
				}

				if (va.State != DependencyState.Loaded)
				{
					yield return dependency.Value.GetDescriptor();
				}

				dependencies[dependency.Key].State = DependencyState.Loaded;
			}
		}

		struct DescriptorValidation
		{
			/// <summary>
			/// Dependency frame state.
			/// </summary>
			public DependencyState State { get; set; }
		}

		#endregion

	}
}
