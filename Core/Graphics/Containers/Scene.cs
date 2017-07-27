using pEngine.Common.DataModel;
using pEngine.Common.Timing.Base;

using pEngine.Core.Graphics.Containers;
using pEngine.Common.Math;

namespace pEngine.Core.Graphics.Containers
{
    public class Scene : Container
    {

		/// <summary>
		/// Creates a new instance of <see cref="Scene"/>.
		/// </summary>
		public Scene()
		{
			ScaleSize = Physics.Axes.Both;
		}

		[LoaderFunction]
		private void Initializer()
		{
			Size = Host.Window.BufferSize;
		}

		public override void Update(IFrameBasedClock clock)
		{
			UpdateSize();

			base.Update(clock);
		}

		private void UpdateSize()
		{
			if (ScaleSize.HasFlag(Physics.Axes.X))
				Size = new Vector2i(Host.Window.BufferSize.X, (int)Size.Y);

			if (ScaleSize.HasFlag(Physics.Axes.Y))
				Size = new Vector2i((int)Size.X, Host.Window.BufferSize.Y);
		}
	}
}
