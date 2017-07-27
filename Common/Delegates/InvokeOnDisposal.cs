using System;

namespace pEngine.Common.Invocation
{
	public class InvokeOnDisposal : IDisposable
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Invocation.InvokeOnDisposal"/> class.
		/// This class allow to call an action when this object is disposed.
		/// </summary>
		/// <param name="action">Action to call.</param>
		public InvokeOnDisposal(Action action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			Action = action;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Invocation.InvokeOnDisposal"/> class.
		/// This class allow to call an action when this object is disposed.
		/// </summary>
		public InvokeOnDisposal()
		{
		}

		/// <summary>
		/// Action to call on dispose.
		/// </summary>
		public Action Action { get; set; }

		/// <summary>
		/// Releases all resource used by the <see cref="T:Invocation.InvokeOnDisposal"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Invocation.InvokeOnDisposal"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="T:Invocation.InvokeOnDisposal"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="T:Invocation.InvokeOnDisposal"/>
		/// so the garbage collector can reclaim the memory that the <see cref="T:Invocation.InvokeOnDisposal"/> was occupying.</remarks>
		public void Dispose()
		{
			if (Action == null)
				throw new InvalidOperationException($"The specified action is null.");

			Action();
		}

	}
}
