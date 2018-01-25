using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Resources;
using pEngine.Framework.Timing;
using pEngine.Framework.Timing.Base;

namespace pEngine.Framework
{
	/// <summary>
	/// Base class for all objects used in a game.
	/// </summary>
	public abstract partial class GameObject : Resource, IUpdatable
	{
		static long id = 0;

		/// <summary>
		/// Makes a new instance of <see cref="GameObject"/> class.
		/// </summary>
		public GameObject(GameObject parent)
		{
			initializeCache();

			Identifier = id++;
		}

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		/// <param name="disposing">Dispose managed resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				disposeCache();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Unique <see cref="GameObject"/> identifier.
		/// </summary>
		public long Identifier { get; }

		/// <summary>
		/// Current object's parent.
		/// </summary>
		public GameObject Parent { get; }

		/// <summary>
		/// Called on state update.
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		protected abstract void OnUpdate(IFrameBasedClock clock);

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public void Update(IFrameBasedClock clock)
		{
			OnUpdate(clock);
		}

		#region .Net overrides

		/// <summary>
		/// Check if two object are equals.
		/// </summary>
		/// <param name="obj">Second object.</param>
		/// <returns>True if are equals.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (GetType() != obj.GetType())
				return false;

			GameObject o = obj as GameObject;

			if (o.Identifier == Identifier)
				return true;

			return false;
		}

		/// <summary>
		/// String rappresentation of a gameobject.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return $"{GetType().Name} #{Identifier}";
		}

		/// <summary>
		/// Get an hash code for game objects.
		/// </summary>
		/// <returns>Hash for this object.</returns>
		public override int GetHashCode()
		{
			return (int)checked(Identifier % int.MaxValue);
		}

		#endregion
	}
}
