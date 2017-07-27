using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Data.FrameDependency
{
    public class FrameDependencyManager<DependencyType, DescriptorType> : IDisposable
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
		public virtual void AddDependency(DependencyType dependency)
		{
			dependency.DependencyID = key;

			if (dependencies.ContainsKey(dependency.DependencyID))
				return;

			validation[key] = new DescriptorValidation
			{ LoadFrame = -1, UpdateFrame = 0 };

			dependencies.TryAdd(key++, dependency);
		}

		/// <summary>
		/// Remove a dependency to the manager.
		/// </summary>
		/// <param name="dependency">Dependency to remove.</param>
		public virtual void RemoveDependency(DependencyType dependency)
		{ 
			DescriptorValidation d;
			DependencyType t;
			validation.TryRemove(dependency.DependencyID, out d);
			dependencies.TryRemove(dependency.DependencyID, out t);
		}

		#endregion

		#region Descriptors

		/// <summary>
		/// Contains all descriptor status.
		/// </summary>
		private ConcurrentDictionary<long, DescriptorValidation> validation;

		/// <summary>
		/// Set a dependency as loaded from the loader thread.
		/// </summary>
		/// <param name="frame">Frame when is loaded.</param>
		public virtual void SetDependencyLoaded(long descriptorId, long frame)
		{
			if (!validation.ContainsKey(descriptorId))
				throw new InvalidOperationException("This dependency is not avaiable.");

			long update = validation[descriptorId].UpdateFrame;

			validation[descriptorId] = new DescriptorValidation
			{ UpdateFrame = update, LoadFrame = frame };
		}

		/// <summary>
		/// Gets all pending dependencies.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<DescriptorType> GetDependencyDescriptors()
		{
			foreach (var dependency in dependencies)
			{
				if (dependency.Value.Invalidated)
				{
					DescriptorValidation v = validation[dependency.Key];
					v.UpdateFrame = host.PhysicsLoop.FrameId;
					validation[dependency.Key] = v;
					dependency.Value.Invalidated = false;
				}

				var va = validation[dependency.Key];

				if (va.LoadFrame < va.UpdateFrame)
				{
					yield return dependency.Value.GetDescriptor();
				}

				dependencies[dependency.Key].Invalidated = false;
			}
		}

		struct DescriptorValidation
		{
			/// <summary>
			/// Last frame when the object is updated.
			/// </summary>
			public long UpdateFrame { get; set; }

			/// <summary>
			/// Last frame when the object is loaded.
			/// </summary>
			public long LoadFrame { get; set; }
		}

		#endregion

	}
}
