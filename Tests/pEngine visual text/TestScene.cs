using System.Collections.Generic;

using pEngine;

using pEngine.Core.Graphics.Containers;
using pEngine.Core.Graphics.Drawables;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Fonts;

using pEngine.Core.Audio;

using pEngine.Common.DataModel;
using pEngine.Common.Timing.Base;

namespace pEngineVisualText
{

	public class TestScene : Scene
	{
		Paragraph mamma;

		[LoaderFunction]
		private void Initializer(TextureStore Textures)
		{
			DynamicFont s = new DynamicFont("DejaVuSansCondensed.ttf", 100, false);
			s.Charsets = new List<KeyValuePair<int, int>>
			{
				Charset.BasicLatin
			};
			s.Load(Game);

			StaticTextureAtlas a = new StaticTextureAtlas();

			a.LoadFromFiles
			(
				("Immagine", "Sfondo2.jpg"),
				("ImmagineSeconda", "Sfondo.jpg")
			);

			a.Load(Game);

			Children.Add(new Sprite(a["Immagine"])
			{
				Size = Size,
				ScaleSize = pEngine.Core.Physics.Axes.Both,
				Stretch = StretchMode.Uniform

			}.Load(Game));


			Children.Add((mamma = new Paragraph(s)
			{
				Text = $@"Test",
				Color = Color4.Aqua,
				Position = new Vector2i(10, 10),
			}.Load<Paragraph>(Game)));

			mamma.Underline = new Underline()
			{
				Enabled = false,
				Color = Color4.Aqua,
				Thickness = 1
			};
		}


		public override void Update(IFrameBasedClock clock)
		{
			//if (s <= 0)
			{
				//mamma.Text = $@"{Host.GraphicsLoop.Clock.FramesPerSecond} FPS";
			}

			
			base.Update(clock);

		}
	}
}
