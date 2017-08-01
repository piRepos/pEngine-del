using pEngine.Common.DataModel;
using pEngine.Core;

using pEngine;

namespace pEngineVisualTest
{
	class VisualTest : Game
	{

		[LoaderFunction]
		private void Initializer()
		{
			Host.Window.Size = new Vector2i(1920, 1080);

			TestScene currScene = new TestScene();

			currScene.Load<TestScene>(this);

			Background.PullScene(currScene);
			
		}


		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
