using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Graphics.Containers
{
	public class Layer : Container<Scene>
	{
		/// <summary>
		/// Makes a new instance of <see cref="Layer"/> class.
		/// </summary>
		public Layer()
		{

		}

		/// <summary>
		/// Implements resource loading.
		/// </summary>
		protected override void OnLoad()
		{

		}

		/// <summary>
		/// Running game.
		/// </summary>
		public Game Game => Parent as Game;

		/// <summary>
		/// Current running scene.
		/// </summary>
		public Scene CurrentScene { get; private set; }

		/// <summary>
		/// Pending loading scenes.
		/// </summary>
		public Scene LoadingScene { get; private set; }

		#region Scene managing

		public void TransitionTo(Scene scene)
		{
			LoadingScene = scene;

			LoadingScene.Loaded += (res) =>
			{
				CurrentScene = res as Scene;
			};

			// TODO: mettere lo scheduler
			LoadingScene.Load();
		}

		public void TransitionToAsync(Scene scene)
		{
			LoadingScene = scene;

			LoadingScene.Loaded += (res) =>
			{
				CurrentScene = res as Scene;
			};

			LoadingScene.LoadAsync();
		}


		#endregion

	}
}
