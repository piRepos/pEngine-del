using System.Collections.Generic;

using pEngine;

using pEngine.Core.Graphics.Containers;
using pEngine.Core.Graphics.Drawables;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Fonts;

using pEngine.Core.Audio;

using pEngine.Common.DataModel;
using pEngine.Common.Timing.Base;

namespace pEngineVisualTest
{

	public class TestScene : Scene
	{
		Paragraph mamma;
		LayerMask mask;
		Sprite background;
		Box clip;

        [LoaderFunction]
        private void Initializer(TextureStore Textures, FontStore Fonts)
        {
            #region Fonts

            DynamicFont comfortaaRegular120 = new DynamicFont("Resources/Comfortaa-Regular.ttf", 120, false);
            DynamicFont comfortaaRegular70 = new DynamicFont("Resources/Comfortaa-Regular.ttf", 70, false);
            DynamicFont comfortaaRegular40 = new DynamicFont("Resources/Comfortaa-Regular.ttf", 40, false);
            DynamicFont comfortaaLight = new DynamicFont("Resources/Comfortaa-Light.ttf", 70, false);
            DynamicFont comfortaaBold = new DynamicFont("Resources/Comfortaa-Bold.ttf", 120, false);

            comfortaaRegular120.Charsets.Add(Charset.BasicLatin);
            comfortaaRegular70.Charsets.Add(Charset.BasicLatin);
            comfortaaRegular40.Charsets.Add(Charset.BasicLatin);
            comfortaaLight.Charsets.Add(Charset.BasicLatin);
            comfortaaBold.Charsets.Add(Charset.BasicLatin);

			comfortaaRegular120.Load(Game);
            comfortaaRegular70.Load(Game);
			comfortaaRegular40.Load(Game);
            comfortaaLight.Load(Game);
            comfortaaBold.Load(Game);

            #endregion

            #region Textures

            StaticTextureAtlas a = new StaticTextureAtlas();
            a.LoadFromFiles
            (
                ("Background", "Resources/pEngineBackground.jpg")
            );
            a.Load(Game);

			#endregion

			#region Background

			Children.Add((clip = new Box
			{
				Size = Size,
				ScaleSize = pEngine.Core.Physics.Axes.Both,
				Children =
				{
					(background = new Sprite(a["Background"])
					{
						Size = Size,
						ScaleSize = pEngine.Core.Physics.Axes.Both,
						Stretch = StretchMode.UniformToFill
					}.Load<Sprite>(Game))
				}
			}).Load<Box>(Game));

			#endregion

			#region pEngine title

			Children.Add((mamma = new Paragraph(comfortaaLight)
			{
				Text = $@"2D game engine.",
				//FrameBuffered = true,
				Color = Color4.White,
				Position = new Vector2i(580, 75),
				ScaleWithParent = true
			}.Load<Paragraph>(Game)));


			Children.Add(new Paragraph(comfortaaRegular40)
			{
				Text = $@"Developed by Andrea Demontis 2017",
				//FrameBuffered = true,
				Color = Color4.White,
				Position = new Vector2i(20, 165),
				ScaleWithParent = true
			}.Load<Paragraph>(Game));


			Children.Add((mask = new LayerMask()
			{
				Size = Size,
				Children =
				{
					new Paragraph(comfortaaRegular120)
					{
						Text = $@"pEngine",
						//FrameBuffered = false,
						Color = Color4.White,
						Position = new Vector2i(200, 300),
						ScaleWithParent = true
					}.Load<Paragraph>(Game)
				}

			}.Load<LayerMask>(Game)));

			clip.AddMask(mask, pEngine.Core.Graphics.Renderer.Clipping.MaskOperation.Add);

            #endregion
        }


		public override void Update(IFrameBasedClock clock)
		{
			
			base.Update(clock);

		}
	}
}
